
namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.PlayerTarget, "Handle Player Target Change.", eClientStatus.PlayerInGame)]
    public class PlayerTargetHandler : IPacketHandler
    {
        public void HandlePacket(GameClient client, GSPacketIn packet)
        {
            ushort targetID = packet.ReadShort();
            ushort flags = packet.ReadShort();

            /*
             * 0x8000 = 'examine' bit
             * 0x4000 = LOS1 bit; is 0 if no LOS
             * 0x2000 = LOS2 bit; is 0 if no LOS
             * 0x0001 = players attack mode bit (not targets!)
             */

            ChangeTarget(client.Player, targetID, (flags & (0x4000 | 0x2000)) != 0, (flags & 0x8000) != 0);
        }

        private static void ChangeTarget(GamePlayer actionSource, ushort newTargetId, bool targetInView, bool examineTarget)
        {
            GameObject target = actionSource.CurrentRegion.GetObject(newTargetId);

            if (target != null && !actionSource.IsWithinRadius(target, WorldMgr.OBJ_UPDATE_DISTANCE))
            {
                actionSource.Out.SendObjectDelete(newTargetId);
                target = null;
            }

            actionSource.TargetObject = target;
            actionSource.TargetInView = targetInView;

            if (target != null)
            {
                // Send target message text only if 'examine' bit is set.
                if (examineTarget)
                {
                    foreach (string message in target.GetExamineMessages(actionSource))
                        actionSource.Out.SendMessage(message, eChatType.CT_Missed, eChatLoc.CL_SystemWindow);
                }

                // No LOS message. Not sure which bit to use so use both.
                if (!targetInView)
                    actionSource.Out.SendMessage("Target is not in view.", eChatType.CT_System, eChatLoc.CL_SystemWindow);

                if (target is not GamePlayer)
                    actionSource.Out.SendObjectUpdate(target);
            }

            if (actionSource.IsPraying)
            {
                if (target is not GameGravestone gravestone || !gravestone.InternalID.Equals(actionSource.InternalID))
                {
                    actionSource.Out.SendMessage("You are no longer targetting your grave. Your prayers fail.", eChatType.CT_System, eChatLoc.CL_SystemWindow);
                    actionSource.PrayTimerStop();
                }
            }
        }
    }
}
