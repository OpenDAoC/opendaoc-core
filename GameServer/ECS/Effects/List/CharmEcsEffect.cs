﻿using System.Linq;
using DOL.AI.Brain;
using DOL.Events;
using DOL.GS.PacketHandler;
using DOL.GS.Spells;
using DOL.Language;

namespace DOL.GS
{
    public class CharmEcsEffect : ECSGameSpellEffect
    {
        public CharmEcsEffect(ECSGameEffectInitParams initParams)
            : base(initParams) { }

        public override void OnStartEffect()
        {
            GamePlayer casterPlayer = SpellHandler.Caster as GamePlayer;
            GameNPC charmMob = Owner as GameNPC;

            if (casterPlayer != null && charmMob != null)
            {
                if (((CharmSpellHandler)SpellHandler).m_controlledBrain == null && !(charmMob.Brain is ControlledNpcBrain))
                {
                    ((CharmSpellHandler)SpellHandler).m_controlledBrain = new ControlledNpcBrain(casterPlayer);
                }
                else
                {
                    ((CharmSpellHandler)SpellHandler).m_controlledBrain = charmMob.Brain as ControlledNpcBrain;
                    ((CharmSpellHandler)SpellHandler).m_isBrainSet = true;
                }

                if (!((CharmSpellHandler)SpellHandler).m_isBrainSet &&
                    !((CharmSpellHandler)SpellHandler).m_controlledBrain.IsActive)
                {

                    charmMob.AddBrain(((CharmSpellHandler)SpellHandler).m_controlledBrain);
                    ((CharmSpellHandler)SpellHandler).m_isBrainSet = true;

                    GameEventMgr.AddHandler(charmMob, GameLivingEvent.PetReleased, (((CharmSpellHandler)SpellHandler).ReleaseEventHandler));
                }

                if (casterPlayer.ControlledBrain != ((CharmSpellHandler)SpellHandler).m_controlledBrain)
                {

                    if (!string.IsNullOrEmpty(SpellHandler.Spell.Message1))
                        // Message: "{0}The slough serpent} is now enthralled!"
                        Message.SystemToArea(charmMob, Util.MakeSentence(SpellHandler.Spell.Message1, charmMob.GetName(0, true)), eChatType.CT_System, charmMob, casterPlayer);

                    if (!string.IsNullOrEmpty(SpellHandler.Spell.Message2))
                        // Message: {0} is now under your control.
                        ((CharmSpellHandler)SpellHandler).MessageToCaster(Util.MakeSentence(SpellHandler.Spell.Message2, charmMob.GetName(0, true)), eChatType.CT_Spell);
                    else
                        // Message: {0} is now under your control.
                        ((CharmSpellHandler)SpellHandler).MessageToCaster(LanguageMgr.GetTranslation(casterPlayer.Client, "GamePlayer.GamePet.StartSpell.UnderControl", charmMob.GetName(0, true)), eChatType.CT_Spell);
                    
                    casterPlayer.SetControlledBrain(((CharmSpellHandler)SpellHandler).m_controlledBrain);

                    foreach (GamePlayer player in charmMob.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                    {
                        player.Out.SendNPCCreate(charmMob);

                        if (charmMob.Inventory != null)
                            player.Out.SendLivingEquipmentUpdate(charmMob);

                        // 'SendNPCCreate' should already call 'SendObjectGuildID'
                        //player.Out.SendObjectGuildID(charmMob, casterPlayer.Guild);
                    }
                }
                ((CharmSpellHandler)SpellHandler).SendEffectAnimation(charmMob, 0, false, 1);
            }
        }

        public override void OnStopEffect()
        {
            GamePlayer casterPlayer = SpellHandler.Caster as GamePlayer;
            GameNPC charmMob = Owner as GameNPC;
            CharmSpellHandler charmSpellHandler = SpellHandler as CharmSpellHandler;

            if (casterPlayer != null && charmMob != null)
            {
                GameEventMgr.RemoveHandler(charmMob, GameLivingEvent.PetReleased, charmSpellHandler.ReleaseEventHandler);
                ControlledNpcBrain oldBrain = (ControlledNpcBrain)casterPlayer.ControlledBrain;
                casterPlayer.SetControlledBrain(null);

                lock (charmMob.BrainSync)
                {
                    var immunityEffects = charmMob.effectListComponent.GetSpellEffects().Where(e => e.TriggersImmunity).ToArray();

                    for (int i = 0; i < immunityEffects.Length; i++)
                        EffectService.RequestImmediateCancelEffect(immunityEffects[i]);

                    charmMob.StopAttack();
                    charmMob.StopCurrentSpellcast();
                    charmMob.RemoveBrain(oldBrain);

                    StandardMobBrain newBrain = new();
                    charmMob.AddBrain(newBrain);

                    charmSpellHandler.m_isBrainSet = false;

                    if (newBrain is IOldAggressiveBrain)
                    {
                        newBrain.ClearAggroList();

                        if (SpellHandler.Spell.Pulse != 0 &&
                            SpellHandler.Caster.ObjectState == GameObject.eObjectState.Active &&
                            SpellHandler.Caster.IsAlive &&
                            !SpellHandler.Caster.IsStealthed)
                        {
                            newBrain.FSM.SetCurrentState(eFSMStateType.AGGRO);
                            newBrain.AddToAggroList(SpellHandler.Caster, SpellHandler.Caster.Level * 10);
                            charmMob.StartAttack(SpellHandler.Caster);
                            charmMob.LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
                        }
                        else if (charmMob.IsWithinRadius(charmMob.SpawnPoint, 5000))
                            charmMob.ReturnToSpawnPoint();
                        else
                        {
                            newBrain.Stop();
                            charmMob.Die(null);
                        }
                    }
                }

                // remove NPC with new brain from all attackers aggro list
                lock (charmMob.attackComponent.Attackers)
                {
                    foreach (GameObject attacker in charmMob.attackComponent.Attackers)
                    {
                        if (attacker is GameNPC npcAttacker && npcAttacker.Brain is IOldAggressiveBrain aggressiveBrain)
                        {
                            aggressiveBrain.RemoveFromAggroList(charmMob);
                            aggressiveBrain.AddToAggroList(casterPlayer, casterPlayer.Level * 10);
                            npcAttacker.StartAttack(casterPlayer);
                            npcAttacker.LastAttackedByEnemyTickPvE = GameLoop.GameLoopTime;
                        }
                    }
                }

                charmSpellHandler.m_controlledBrain?.ClearAggroList();
                charmMob.StopFollowing();
                charmMob.TempProperties.setProperty(GameNPC.CHARMED_TICK_PROP, charmMob.CurrentRegion.Time);

                foreach (GamePlayer ply in charmMob.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
                {
                    if (charmMob.IsAlive)
                    {
                        ply.Out.SendNPCCreate(charmMob);

                        if (charmMob.Inventory != null)
                            ply.Out.SendLivingEquipmentUpdate(charmMob);

                        ply.Out.SendObjectGuildID(charmMob, null);
                    }
                }
            }

            ECSPulseEffect song = EffectListService.GetPulseEffectOnTarget(casterPlayer, SpellHandler.Spell);

            if (charmMob != null && song != null)
                EffectService.RequestImmediateCancelConcEffect(song);

            charmSpellHandler.m_controlledBrain = null;
        }
    }
}
