using System.Collections.Generic;
using DOL.Database;
using DOL.GS.Effects;

namespace DOL.GS.Spells
{
	public interface ISpellHandler
	{
		eCastState CastState { get; set; }
		GameLiving Target { get; set; }
		bool HasLos { get; set; }

		void CreateECSEffect(EcsGameEffectInitParams initParams);

		/// <summary>
		/// Starts the spell, without displaying cast message etc.
		/// Should be used for StyleEffects, ...
		/// </summary>
		bool StartSpell(GameLiving target);
		
		/// <summary>
		/// Starts the spell, without displaying cast message etc.
		/// Should be used with spells attached to items (procs, /use, etc)
		/// </summary>
		bool StartSpell(GameLiving target, DbInventoryItem item);

		/// <summary>
		/// Whenever the current casting sequence is to be interrupted
		/// this callback is called
		/// </summary>
		void InterruptCasting();

		/// <summary>
		/// Has to be called when the caster moves
		/// </summary>
		void CasterMoves();

		/// <summary>
		/// Has to be called when the caster is attacked by enemy
		/// for interrupt checks
		/// <param name="attacker">attacker that interrupts the cast sequence</param>
		/// <returns>true if casting was interrupted</returns>
		/// </summary>
		bool CasterIsAttacked(GameLiving attacker);

		/// <summary>
		/// Returns true when spell is in casting phase
		/// </summary>
		bool IsInCastingPhase { get; }

		/// <summary>
		/// Can this spell be queued with other spells?
		/// </summary>
		bool CanQueue { get; }

		/// <summary>
		/// Does this spell break stealth on start of cast?
		/// </summary>
		bool UnstealthCasterOnStart {get; }

		/// <summary>
		/// Does this spell break stealth on Finish of cast?
		/// </summary>
		bool UnstealthCasterOnFinish { get; }
		
		/// <summary>
		/// Should we start the reuse timer (spell successful)?
		/// </summary>
		bool StartReuseTimer { get; }

		/// <summary>
		/// Gets wether this spell has positive or negative impact on targets
		/// important to determine wether the spell can be canceled by a player
		/// </summary>
		/// <returns></returns>
		bool HasPositiveEffect { get; }

		/// <summary>
		/// Gets wether this spellis Purgeable or not
		/// important for Masterlevels since they aren't purgeable
		/// </summary>
		/// <returns></returns>
		bool IsUnPurgeAble { get; }

		/// <summary>
		/// Determines wether new spell is better than existing one
		/// important for overwriting
		/// </summary>
		/// <param name="oldeffect"></param>
		/// <param name="neweffect"></param>
		/// <returns>true if new spell is better version</returns>
		bool IsNewEffectBetter(GameSpellEffect oldeffect, GameSpellEffect neweffect);

		/// <summary>
		/// Determines wether this spell is compatible with given spell
		/// and therefore overwritable by better versions
		/// spells that are overwritable do not stack
		/// </summary>
		/// <param name="compare"></param>
		/// <returns></returns>
		bool IsOverwritable(EcsGameSpellEffect compare);
		/// <summary>
		/// Determines wether new spell is better than old spell and should disable it
		/// </summary>
		/// <param name="oldeffect"></param>
		/// <param name="neweffect"></param>
		/// <returns></returns>
		bool IsCancellableEffectBetter(GameSpellEffect oldeffect, GameSpellEffect neweffect);
		
		/// <summary>
		/// Determines wether this spell can be disabled
		/// by better versions spells that stacks without overwriting
		/// </summary>
		/// <param name="compare"></param>
		/// <returns></returns>
		bool IsCancellable(GameSpellEffect compare);
		
		/// <summary>
		/// Can this SpellHandler Coexist with other Overwritable Spell Effect
		/// </summary>
		bool AllowCoexisting { get; }

		/// <summary>
		/// Does this spell ignore all damage caps?
		/// </summary>
		bool IgnoreDamageCap { get; set; }

	    long CastStartTick { get; }
		/// <summary>
		/// Should this spell use the minimum variance for the type?
		/// Followup style effects, for example, always use the minimum variance
		/// </summary>
		bool UseMinVariance { get; set; }

		/// <summary>
		/// Actions to take when the effect starts
		/// </summary>
		/// <param name="effect"></param>
		void OnEffectStart(GameSpellEffect effect);

		/// <summary>
		/// Actions to take when the effect stops
		/// </summary>
		/// <param name="effect"></param>
		void OnEffectPulse(GameSpellEffect effect);

		/// <summary>
		/// When an applied effect expires.
		/// Duration spells only.
		/// </summary>
		/// <param name="effect">The expired effect</param>
		/// <param name="noMessages">true, when no messages should be sent to player and surrounding</param>
		/// <returns>immunity duration in milliseconds</returns>
		int OnEffectExpires(GameSpellEffect effect, bool noMessages);

		/// <summary>
		/// When spell pulses
		/// </summary>
		/// <param name="effect">The effect doing the pulses</param>
		void OnSpellPulse(PulsingSpellEffect effect);

		/// <summary>
		/// Effect from Spell is Added to Living Effect List
		/// </summary>
		/// <param name="effect"></param>
		void OnEffectAdd(GameSpellEffect effect);
		
		/// <summary>
		/// Effect from Spell is Removed from Living Effect List
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="overwrite"></param>
		void OnEffectRemove(GameSpellEffect effect, bool overwrite);
		
		/// <summary>
		/// The Spell Caster
		/// </summary>
		GameLiving Caster { get; }

		void Tick(long currentTick);

		/// <summary>
		/// The power cost for this spell.
		/// </summary>
		/// <param name="caster"></param>
		/// <returns></returns>
		int PowerCost(GameLiving caster);

		/// <summary>
		/// The ability casting the spell
		/// </summary>
		ISpellCastingAbilityHandler Ability { get; set; }

		/// <summary>
		/// The Spell
		/// </summary>
		Spell Spell { get; }

		/// <summary>
		/// The SpellLine
		/// </summary>
		SpellLine SpellLine { get; }

		/// <summary>
		/// The DelveInfo
		/// </summary>
		IList<string> DelveInfo { get; }

		/// <summary>
		/// Current depth of delve info
		/// </summary>
		byte DelveInfoDepth { get; set; }

		DbPlayerXEffect GetSavedEffect(GameSpellEffect e);
		void OnEffectRestored(GameSpellEffect effect, int[] RestoreVars);
		int OnRestoredEffectExpires(GameSpellEffect effect, int[] RestoreVars, bool noMessages);
		bool CheckBeginCast(GameLiving selectedTarget);
		
		/// <summary>
		/// Calculates the range to target needed to cast the spell
		/// </summary>
		/// <returns>Modified Spell Range</returns>
		int CalculateSpellRange();
		void TooltipDelve(ref DOL.GS.PacketHandler.MiniDelveWriter dw);
	}

	/// <summary>
	/// Callback when spell handler has done its cast work
	/// </summary>
	public delegate void CastingCompleteCallback(ISpellHandler handler);

	/// <summary>
	/// Callback when spell handler is completely done and duration spell expired
	/// or concentration spell was canceled
	/// </summary>
	public delegate void SpellEndsCallback(ISpellHandler handler);
}
