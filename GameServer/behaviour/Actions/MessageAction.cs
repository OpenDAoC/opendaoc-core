using System;
using System.Collections.Generic;
using System.Text;
using DOL.GS.PacketHandler;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.Behaviour;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.Message)]
    public class MessageAction : AbstractAction<String,eTextType>
    {

        public MessageAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.Message, p, q)
        {                           
        }


        public MessageAction(GameNPC defaultNPC, String message, eTextType messageType)
            : this(defaultNPC, (object)message, (object)messageType) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            String message = BehaviourUtils.GetPersonalizedMessage(P, player);
            switch (Q)
            {
                case eTextType.Dialog:
                    player.Out.SendCustomDialog(message, null);
                    break;
                case eTextType.Emote:
                    player.Out.SendMessage(message, eChatType.CT_Emote, eChatLoc.CL_ChatWindow);
                    break;
				case eTextType.Say:
					player.Out.SendMessage(message, eChatType.CT_Say, eChatLoc.CL_ChatWindow);
					break;
				case eTextType.SayTo:
					player.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_PopupWindow);
					break;
				case eTextType.Yell:
					player.Out.SendMessage(message, eChatType.CT_Help, eChatLoc.CL_ChatWindow);
					break;
                case eTextType.Broadcast:
                    foreach (GameClient clientz in WorldMgr.GetAllPlayingClients())
                    {
                        clientz.Player.Out.SendMessage(message, eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                    }
                    break;
                case eTextType.Read:
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.MessageAction.ReadMessage", message), eChatType.CT_Emote, eChatLoc.CL_PopupWindow);
                    break;  
                case eTextType.None:
                    //nohting
                    break;
            }
        }
    }
}
