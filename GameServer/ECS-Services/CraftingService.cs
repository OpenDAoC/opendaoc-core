﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ECS.Debug;

namespace DOL.GS
{
    public static class CraftingService
    {
        private static readonly Logging.Logger log = Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);
        private const string SERVICE_NAME = nameof(CraftingService);
        private static List<CraftComponent> _list;
        private static int _entityCount;

        public static void Tick()
        {
            GameLoop.CurrentServiceTick = SERVICE_NAME;
            Diagnostics.StartPerfCounter(SERVICE_NAME);
            _list = EntityManager.UpdateAndGetAll<CraftComponent>(EntityManager.EntityType.CraftComponent, out int lastValidIndex);
            Parallel.For(0, lastValidIndex + 1, TickInternal);

            if (Diagnostics.CheckEntityCounts)
                Diagnostics.PrintEntityCount(SERVICE_NAME, ref _entityCount, _list.Count);

            Diagnostics.StopPerfCounter(SERVICE_NAME);
        }

        private static void TickInternal(int index)
        {
            CraftComponent craftComponent = _list[index];

            try
            {
                if (craftComponent?.EntityManagerId.IsSet != true)
                    return;

                if (Diagnostics.CheckEntityCounts)
                    Interlocked.Increment(ref _entityCount);

                craftComponent.Tick();
            }
            catch (Exception e)
            {
                ServiceUtils.HandleServiceException(e, SERVICE_NAME, craftComponent, craftComponent.Owner);
            }
        }
    }
}
