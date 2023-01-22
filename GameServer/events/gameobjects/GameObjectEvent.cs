/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using DOL.GS;

namespace DOL.Events;

/// <summary>
/// This class holds all possible GameObject events.
/// Only constants defined here!
/// </summary>
public class GameObjectEvent : DOLEvent
{
    /// <summary>
    /// Constructs a new GameObjectEvent
    /// </summary>
    /// <param name="name">the name of the event</param>
    protected GameObjectEvent(string name) : base(name)
    {
    }

    /// <summary>
    /// Tests if this event is valid for the specified object
    /// </summary>
    /// <param name="o">The object for which the event wants to be registered</param>
    /// <returns>true if valid, false if not</returns>
    public override bool IsValidFor(object o)
    {
        return o is GameObject;
    }

    /// <summary>
    /// The AddToWorld event is fired whenever the object is added to the world
    /// </summary>
    public static readonly GameObjectEvent AddToWorld = new("GameObject.AddToWorld");

    /// <summary>
    /// The RemoveFromWorld event is fired whenever the object is removed from the world
    /// </summary>
    public static readonly GameObjectEvent RemoveFromWorld = new("GameObject.RemoveFromWorld");

    /// <summary>
    /// The MoveTo event is fired whenever the object is moved to a new position by the MoveTo method
    /// <seealso cref="MoveToEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent MoveTo = new("GameObject.MoveTo");

    /// <summary>
    /// The Delete event is fired whenever the object is deleted
    /// </summary>
    public static readonly GameObjectEvent Delete = new("GameObject.Delete");

    /// <summary>
    /// The Interact event is fired whenever a player interacts with this object
    /// <seealso cref="InteractEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent Interact = new("GameObject.Interact");

    /// <summary>
    /// The Interact Failed event is fired whenever a player interacts with this object but fails
    /// <seealso cref="InteractEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent InteractFailed = new("GameObject.InteractFailed");

    /// <summary>
    /// The Interact event is fired whenever a player interacts with something
    /// <seealso cref="InteractEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent InteractWith = new("GameObject.InteractWith");

    /// <summary>
    /// The ReceiveItem event is fired whenever the object receives an item
    /// <seealso cref="ReceiveItemEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent ReceiveItem = new("GameObjectEvent.ReceiveItem");

    /// <summary>
    /// The ReceiveMoney event is fired whenever the object receives money
    /// <seealso cref="ReceiveMoneyEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent ReceiveMoney = new("GameObjectEvent.ReceiveMoney");

    /// <summary>
    /// The TakeDamage event is fired whenever an object takes damage
    /// <seealso cref="TakeDamageEventArgs"/>
    /// </summary>
    public static readonly GameObjectEvent TakeDamage = new("GameObject.TakeDamage");

    /// <summary>
    /// The FinishedLosCheck event is fired whenever a LoS Check is finished.
    /// </summary>
    public static readonly GameObjectEvent FinishedLosCheck = new("GameObject.FinishLosCheck");
}