using System;
using Core.GS.Spells;

namespace Core.GS.Events;

/// <summary>
/// Holds the arguments for the CastSpell event of GameLivings
/// </summary>
public class CastingEventArgs : EventArgs
{

	/// <summary>
	/// The Spell Handler
	/// </summary>
	private ISpellHandler m_handler;

	/// <summary>
	/// The target of the spell
	/// </summary>
	private GameLiving m_target = null;

	/// <summary>
	/// The AttackData generated by this attack, if any
	/// </summary>
	private AttackData m_lastAttackData = null;

	/// <summary>
	/// Constructs a new cast event args
	/// </summary>
	public CastingEventArgs(ISpellHandler handler)
	{
		this.m_handler = handler;
	}

	/// <summary>
	/// Constructs a new cast event args
	/// </summary>
	public CastingEventArgs(ISpellHandler handler, GameLiving target)
	{
		this.m_handler = handler;
		this.m_target = target;
	}

	public CastingEventArgs(ISpellHandler handler, GameLiving target, AttackData ad)
	{
		this.m_handler = handler;
		this.m_target = target;
		m_lastAttackData = ad;
	}

	/// <summary>
	/// Gets the handler
	/// </summary>
	public ISpellHandler SpellHandler
	{
		get { return m_handler; }
	}

	/// <summary>
	/// Gets the target
	/// </summary>
	public GameLiving Target
	{
		get { return m_target; }
	}

	/// <summary>
	/// Get the last AttackData result for this spell cast, if any
	/// </summary>
	public AttackData LastAttackData
	{
		get { return m_lastAttackData; }
	}
}