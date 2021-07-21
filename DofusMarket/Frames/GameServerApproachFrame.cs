using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dofus;
using Dofus.Messages;
using Dofus.Types;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Frames
{
    internal class GameServerApproachFrame : Frame
    {
        private readonly string _ticket;

        public GameServerApproachFrame(string ticket)
        {
            _ticket = ticket;
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

            var charactersList = await ReceiveMessageAsync<CharactersListMessage>();
            CharacterBaseInformations character = charactersList.Characters.First();
            await SendMessageAsync(new CharacterSelectionMessage { Id = character.Id });

            await ReceiveMessageAsync<CharacterSelectedSuccessMessage>();
        }
    }
}
