﻿using DOL.Database;
using DOL.GS;
using DOL.GS.Effects;
using NUnit.Framework;
using System.Collections.Generic;

namespace DOL.Tests.Unit.Gameserver;

[TestFixture]
internal class UT_GameEffectList
{
    [Test]
    public void Add_OwnerIsNotAlive_ReturnFalse()
    {
        var owner = NewFakeLiving();
        owner.fakeIsAlive = false;
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        var actual = effectList.Add(effect);

        Assert.AreEqual(false, actual);
    }

    [Test]
    public void Add_OwnerIsInactiveObject_ReturnFalse()
    {
        var owner = NewFakeLiving();
        owner.fakeIsAlive = true;
        owner.fakeObjectState = GameObject.eObjectState.Inactive;
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        var actual = effectList.Add(effect);

        Assert.AreEqual(false, actual);
    }

    [Test]
    public void Add_OwnerIsActiveObjectAndAlive_ReturnTrue()
    {
        var owner = NewFakeLiving();
        owner.fakeIsAlive = true;
        owner.fakeObjectState = GameObject.eObjectState.Active;
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        var actual = effectList.Add(effect);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public void Add_ToFreshListAndOwnerIsAliveAndActiveObject_ListCountIsOne()
    {
        var owner = NewFakeLiving();
        owner.fakeIsAlive = true;
        owner.fakeObjectState = GameObject.eObjectState.Active;
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        effectList.Add(effect);

        var actual = effectList.Count;
        Assert.AreEqual(1, actual);
    }

    [Test]
    public void Add_ToFreshListAndOwnerIsNotAlive_ListCountRemainsZero()
    {
        var owner = NewFakeLiving();
        owner.fakeIsAlive = false;
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        effectList.Add(effect);

        var actual = effectList.Count;
        Assert.AreEqual(0, actual);
    }

    [Test]
    public void Remove_EffectFromFreshList_ReturnFalse()
    {
        var owner = NewFakeLiving();
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();

        var actual = effectList.Remove(effect);

        Assert.AreEqual(false, actual);
    }

    [Test]
    public void Remove_EffectFromListContainingSameEffect_ReturnTrue()
    {
        var owner = NewFakeLiving();
        var effectList = NewGameEffectList(owner);
        var effect = NewFakeEffect();
        effectList.Add(effect);

        var actual = effectList.Remove(effect);

        Assert.AreEqual(true, actual);
    }

    [Test]
    public void Remove_EffectFromListContainingDifferentEffect_ReturnFalse()
    {
        var effectList = NewGameEffectList();
        var effect = NewFakeEffect();
        var differentEffect = NewFakeEffect();
        effectList.Add(differentEffect);

        var actual = effectList.Remove(effect);

        Assert.AreEqual(false, actual);
    }

    [Test]
    public void Remove_EffectFromListContainingSameEffect_ListCountIsZero()
    {
        var effectList = NewGameEffectList();
        var effect = NewFakeEffect();
        effectList.Add(effect);

        effectList.Remove(effect);

        var actual = effectList.Count;
        Assert.AreEqual(0, actual);
    }

    [Test]
    public void CancelAll_EffectContainsOneEffect_EffectIsCancelled()
    {
        var effectList = NewGameEffectList();
        var effect = NewFakeEffect();
        effectList.Add(effect);

        effectList.CancelAll();

        Assert.IsTrue(effect.receivedCancel);
    }

    [Test]
    public void OnEffectsChanged_NoOpenChanges_NPCupdatePetWindowIsCalled()
    {
        var brain = NewFakeControlledBrain();
        var owner = new GameNPC(brain);
        var effectList = NewGameEffectList(owner);

        effectList.OnEffectsChanged(null);

        Assert.IsTrue(brain.receivedUpdatePetWindow);
    }

    [Test]
    public void OnEffectsChanged_OpenChanges_NPCupdatePetWindowIsNotCalled()
    {
        var brain = NewFakeControlledBrain();
        var owner = new GameNPC(brain);
        var effectList = NewGameEffectList(owner);

        effectList.BeginChanges();
        effectList.OnEffectsChanged(null);

        Assert.IsFalse(brain.receivedUpdatePetWindow);
    }

    [Test]
    public void CommitChanges_NoOpenChanges_NPCupdatePetWindowIsCalled()
    {
        var brain = NewFakeControlledBrain();
        var owner = new GameNPC(brain);
        var effectList = NewGameEffectList(owner);

        effectList.OnEffectsChanged(null);

        Assert.IsTrue(brain.receivedUpdatePetWindow);
    }

    [Test]
    public void CommitChanges_OpenChanges_NPCupdatePetWindowIsNotCalled()
    {
        var brain = NewFakeControlledBrain();
        var owner = new GameNPC(brain);
        var effectList = NewGameEffectList(owner);

        effectList.BeginChanges();
        effectList.OnEffectsChanged(null);

        Assert.IsFalse(brain.receivedUpdatePetWindow);
    }

    [Test]
    public void GetOfType_FreshList_ReturnNull()
    {
        var owner = NewFakeLiving();
        var effectList = new GameEffectList(owner);

        IGameEffect actual = effectList.GetOfType<GameSpellEffect>();

        Assert.AreEqual(null, actual);
    }

    [Test]
    public void GetOfType_ListWithOneItemOfGivenType_ReturnListWithThatOneItem()
    {
        var effectList = NewGameEffectList();
        var effect = new GameSpellEffect(null, 0, 0);
        effectList.Add(effect);

        IGameEffect actual = effectList.GetOfType<GameSpellEffect>();

        Assert.IsNotNull(actual);
    }

    //RestoreAllEffects calls Database

    //SaveAllEffects calls Database

    private static GameEffectList NewGameEffectList()
    {
        return NewGameEffectList(new FakeNPC());
    }

    private static GameEffectList NewGameEffectList(GameLiving owner)
    {
        return new(owner);
    }

    private static FakeLiving NewFakeLiving()
    {
        return new();
    }

    private static FakeGameEffect NewFakeEffect()
    {
        return new();
    }

    private static FakeControlledBrain NewFakeControlledBrain()
    {
        return new();
    }

    private class FakeGameEffect : IGameEffect
    {
        public bool receivedCancel = false;

        public string Name => throw new System.NotImplementedException();
        public int RemainingTime => throw new System.NotImplementedException();
        public ushort Icon => throw new System.NotImplementedException();

        public ushort InternalID
        {
            get => throw new System.NotImplementedException();
            set { }
        }

        public IList<string> DelveInfo => throw new System.NotImplementedException();

        public void Cancel(bool playerCanceled)
        {
            receivedCancel = true;
        }

        public PlayerXEffect getSavedEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}