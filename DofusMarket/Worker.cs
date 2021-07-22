using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Dofus.Serialization;
using Dofus.Types;
using DofusMarket.Frames;
using DofusMarket.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DofusMarket
{
    internal class Worker : BackgroundService
    {
        private static readonly IPEndPoint DofusConnectionEndpoint = new(IPAddress.Parse("34.252.21.81"), 5555); // connection.host/port in config.xml

        private readonly CryptoService _cryptoService;
        private readonly DofusMetrics _dofusMetrics;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;

        public Worker(CryptoService cryptoService, DofusMetrics dofusMetrics, IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory)
        {
            _cryptoService = cryptoService;
            _dofusMetrics = dofusMetrics;
            _appLifetime = appLifetime;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(typeof(DofusClient));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await InnerExecuteAsync(cancellationToken);
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "A non-recoverable error occured");
                _appLifetime.StopApplication();
            }
        }

        private async Task InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var authenticationResult = await AuthenticateAsync(DofusConnectionEndpoint, cancellationToken);
            if (authenticationResult == null)
            {
                return;
            }

            using DofusClient client = new(authenticationResult.ServerEndPoint, _loggerFactory.CreateLogger(typeof(DofusClient)));
            FrameManager frameManager = new(client, _loggerFactory);

            var gameServerApproachFrame = frameManager.Register(new GameServerApproachFrame(authenticationResult.Ticket));
            await gameServerApproachFrame.ProcessTask;

            // Try to imitate what the official client sends after connecting.
            frameManager.Register(new SocialFrame());
            frameManager.Register(new QuestFrame());
            frameManager.Register(new ChatFrame());
            frameManager.Register(new AllianceFrame());
            frameManager.Register(new WorldFrame());
            frameManager.Register(new SynchronizationFrame());
            frameManager.Register(new ItemPricesCollectorFrame(_dofusMetrics));
            // Send again a hardcoded flash key.
            await client.SendMessageAsync(new ClientKeyMessage { Key = "R7JdEA438imJUeyTlF#01" });
            await client.SendMessageAsync(new GameContextCreateRequestMessage());

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task<AuthenticationResult?> AuthenticateAsync(EndPoint connectionEndpoint, CancellationToken cancellationToken)
        {
            using DofusClient client = new(connectionEndpoint, _loggerFactory.CreateLogger(typeof(DofusClient)));

            using AesManaged aes = new() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
            Debug.Assert(aes.Key.Length == 32);

            await foreach (INetworkMessage message in client.Messages.ReadAllAsync(cancellationToken))
            {
                switch (message)
                {
                    case HelloConnectMessage helloConnect:
                        _logger.LogInformation("Identifying");
                        await IdentifyAsync(helloConnect, aes, client);
                        break;

                    case LoginQueueStatusMessage queueStatus:
                        _logger.LogInformation("In queue ({0}/{1})", queueStatus.Position, queueStatus.Total);
                        break;

                    case IdentificationFailedMessage m:
                        _logger.LogError("Identification failed: {0}", m.Reason);
                        return null;

                    case IdentificationFailedForBadVersionMessage m:
                        _logger.LogError("Identification failed for bad version. Required: {0}", m.RequiredVersion);
                        return null;

                    case IdentificationSuccessMessage:
                        _logger.LogInformation("Identification succeed");
                        break;

                    case ServerListMessage serverList:
                        GameServerInformations server = serverList.Servers.First(s => s.CharactersCount != 0);
                        await client.SendMessageAsync(new ServerSelectionMessage { ServerId = server.Id });
                        break;

                    case SelectedServerRefusedMessage selectedServerRefused:
                        _logger.LogError("Connection refused to server {0}, status={1}, error={2}",
                            selectedServerRefused.ServerId, selectedServerRefused.ServerStatus,
                            selectedServerRefused.Error);
                        return null; // TODO: wait for ServerStatusUpdateMessage to retry.

                    case SelectedServerDataMessage selectedServerData:
                        _logger.LogInformation("Connected to server {0}", selectedServerData.ServerId);
                        string ticket = Encoding.UTF8.GetString(_cryptoService.AesDecrypt(selectedServerData.Ticket, aes));
                        DnsEndPoint serverEndPoint = new(selectedServerData.Address, selectedServerData.Port[0]);
                        return new AuthenticationResult(serverEndPoint, ticket);
                }
            }

            return null;
        }

        private async Task IdentifyAsync(HelloConnectMessage helloConnect, Aes aes, DofusClient client)
        {
            // Taken from the official Dofus client in com.ankamagames.dofus.logic.connection.managers.AuthentificationManager__verifyKey
            const string verifyKeyPem = @"-----BEGIN PUBLIC KEY-----
MIIBUzANBgkqhkiG9w0BAQEFAAOCAUAAMIIBOwKCATIAgucoka9J2PXcNdjcu6CuDmgteIMB+rih
2UZJIuSoNT/0J/lEKL/W4UYbDA4U/6TDS0dkMhOpDsSCIDpO1gPG6+6JfhADRfIJItyHZflyXNUj
WOBG4zuxc/L6wldgX24jKo+iCvlDTNUedE553lrfSU23Hwwzt3+doEfgkgAf0l4ZBez5Z/ldp9it
2NH6/2/7spHm0Hsvt/YPrJ+EK8ly5fdLk9cvB4QIQel9SQ3JE8UQrxOAx2wrivc6P0gXp5Q6bHQo
ad1aUp81Ox77l5e8KBJXHzYhdeXaM91wnHTZNhuWmFS3snUHRCBpjDBCkZZ+CxPnKMtm2qJIi57R
slALQVTykEZoAETKWpLBlSm92X/eXY2DdGf+a7vju9EigYbX0aXxQy2Ln2ZBWmUJyZE8B58CAwEA
AQ==
-----END PUBLIC KEY-----";
            const string accountName = "tresgrossepoubelle";
            const string password = "huif6PPiVmkvLku";
            const uint certificateId = 118484446;
            const string certificateHash = "e1e9f893a82bdca6a50f85cb7d430607f955c7aa7b965add839c50fd9e6e05fa";

            Debug.Assert(helloConnect.Salt.Length == 32);
            byte[] key = _cryptoService.RsaDecrypt(helloConnect.Key, verifyKeyPem);
            string keyPem = string.Join(Environment.NewLine,
                "-----BEGIN PUBLIC KEY-----", Convert.ToBase64String(key), "-----END PUBLIC KEY-----");

            MemoryStream credentialsStream = new();
            using (DofusBinaryWriter credentialsWriter = new(credentialsStream, leaveOpen: true))
            {
                credentialsWriter.Write(Encoding.ASCII.GetBytes(helloConnect.Salt));
                if (certificateHash != null)
                {
                    credentialsWriter.Write(aes.Key);
                    credentialsWriter.Write(certificateId);
                }

                credentialsWriter.Write(Encoding.ASCII.GetBytes(certificateHash));
                credentialsWriter.Write((byte)accountName.Length);
                credentialsWriter.Write(Encoding.ASCII.GetBytes(accountName));
                credentialsWriter.Write(Encoding.ASCII.GetBytes(password));
            }

            byte[] credentials = credentialsStream.ToArray();
            byte[] encryptedCredentials = _cryptoService.RsaEncrypt(credentials, keyPem);

            await client.SendMessageAsync(new IdentificationMessage
            {
                AutoConnect = false,
                UseCertificate = certificateHash != null,
                UseLoginToken = false,
                Version = new DofusVersion { Major = 2, Minor = 60, Code = 1, Build = 6, BuildType = BuildType.Release },
                Lang = "fr",
                Credentials = encryptedCredentials,
                ServerId = 0,
                SessionOptionalSalt = 0,
                FailedAttempts = Array.Empty<short>(),
            });

            // The flash key is supposed to be generated and reused between sessions but let's just hardcode it for now.
            await client.SendMessageAsync(new ClientKeyMessage { Key = "R7JdEA438imJUeyTlF#01" });
        }

        private record AuthenticationResult(EndPoint ServerEndPoint, string Ticket);
    }
}
