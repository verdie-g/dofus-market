using System;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Frames
{
    internal class GameServerApproachFrame : Frame
    {
        private readonly string _ticket;
        private readonly long _characterId;

        public GameServerApproachFrame(string ticket, long characterId)
        {
            _ticket = ticket;
            _characterId = characterId;
        }

        public override async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await ReceiveMessageAsync<HelloGameMessage>();
            await SendMessageAsync(new AuthenticationTicketMessage
            {
                Lang = "fr",
                Ticket = _ticket,
            });

            await ReceiveMessageAsync<RawDataMessage>();
            // RawDataMessage contains a SWF that should get executed to generate a CheckIntegrityMessage.
            // Rumors say that is content is not check anymore so try with random bytes and come here later
            // if we get banned.
            CheckIntegrityMessage checkIntegrity = new() { Data = new byte[256] };
            Random rnd = new();
            rnd.NextBytes(checkIntegrity.Data);
            await SendMessageAsync(checkIntegrity);

            switch (await ReceiveAnyMessageAsync(typeof(AuthenticationTicketRefusedMessage),
                typeof(AuthenticationTicketAcceptedMessage), typeof(AuthenticationTicketAcceptedMessage)))
            {
                    case AuthenticationTicketRefusedMessage:
                        Logger.LogError("Authentication ticket refused");
                        return;

                    case AuthenticationTicketAcceptedMessage:
                        Logger.LogInformation("Authentication ticket accepted");
                        await SendMessageAsync(new CharacterListRequestMessage());
                        break;
            }

            await ReceiveMessageAsync<CharactersListMessage>();
            await SendMessageAsync(new CharacterSelectionMessage { Id = (ulong)_characterId });

            await ReceiveMessageAsync<CharacterSelectedSuccessMessage>();
        }
    }
}
