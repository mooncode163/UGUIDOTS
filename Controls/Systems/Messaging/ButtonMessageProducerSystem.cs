﻿using Unity.Entities;
using UnityEngine;

namespace UGUIDots.Controls.Messaging.Systems {

    [UpdateInGroup(typeof(MessagingConsumptionGroup))]
    public class ButtonMessageConsumerSystem : SystemBase {

        private EntityCommandBufferSystem cmdBufferSystem;

        protected override void OnCreate() {
            cmdBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var cmdBuffer = cmdBufferSystem.CreateCommandBuffer().ToConcurrent();

            Dependency = Entities.ForEach((Entity entity, in ButtonMessageRequest c0) => {
                cmdBuffer.DestroyEntity(entity.Index, entity);
            }).WithBurst().Schedule(Dependency);

            cmdBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }

    [UpdateInGroup(typeof(MessagingProductionGroup))]
    public class ButtonMessageProducerSystem : SystemBase {

        private EntityCommandBufferSystem cmdBufferSystem;
        
        protected override void OnCreate() {
            cmdBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            var cmdBuffer = cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
            Dependency = Entities.ForEach((Entity entity, in ClickState c0, in ButtonArchetypeProducerRequest c1) => {
                if (c0.Value) {
                    var e = cmdBuffer.CreateEntity(entity.Index, c1.Value);
                }
            }).ScheduleParallel(Dependency);

            cmdBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
