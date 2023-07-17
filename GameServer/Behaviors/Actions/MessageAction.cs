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
    [ActionAttribute(ActionType = EActionType.Message)]
    public class MessageAction : AbstractAction<String,ETextType>
    {

        public MessageAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, EActionType.Message, p, q)
        {                           
        }


        public MessageAction(GameNPC defaultNPC, String message, ETextType messageType)
            : this(defaultNPC, (object)message, (object)messageType) { }
        


        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviorUtils.GuessGamePlayerFromNotify(e, sender, args);
            String message = BehaviorUtils.GetPersonalizedMessage(P, player);
            switch (Q)
            {
                case ETextType.Dialog:
                    player.Out.SendCustomDialog(message, null);
                    break;
                case ETextType.Emote:
                    player.Out.SendMessage(message, eChatType.CT_Emote, eChatLoc.CL_ChatWindow);
                    break;
				case ETextType.Say:
					player.Out.SendMessage(message, eChatType.CT_Say, eChatLoc.CL_ChatWindow);
					break;
				case ETextType.SayTo:
					player.Out.SendMessage(message, eChatType.CT_System, eChatLoc.CL_PopupWindow);
					break;
				case ETextType.Yell:
					player.Out.SendMessage(message, eChatType.CT_Help, eChatLoc.CL_ChatWindow);
					break;
                case ETextType.Broadcast:
                    foreach (GameClient clientz in WorldMgr.GetAllPlayingClients())
                    {
                        clientz.Player.Out.SendMessage(message, eChatType.CT_Broadcast, eChatLoc.CL_ChatWindow);
                    }
                    break;
                case ETextType.Read:
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.MessageAction.ReadMessage", message), eChatType.CT_Emote, eChatLoc.CL_PopupWindow);
                    break;  
                case ETextType.None:
                    //nohting
                    break;
            }
        }
    }
}
