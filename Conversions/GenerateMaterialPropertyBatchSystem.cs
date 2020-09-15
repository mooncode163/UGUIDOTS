using UGUIDOTS.Render;
using Unity.Entities;
using UnityEngine;

namespace UGUIDOTS.Conversions.Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class GenerateMaterialPropertyBatchSystem : SystemBase {

        protected override void OnUpdate() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif

            // var renderElements = GetBufferFromEntity<RenderElement>(true);
            // var spans          = GetBufferFromEntity<BatchedSpanElement>(true);
            // var colors         = GetComponentDataFromEntity<AppliedColor>(true);
            // var linkedTextures = GetComponentDataFromEntity<LinkedTextureEntity>(true);

            Entities.ForEach((Entity entity, DynamicBuffer<SubmeshKeyElement> b0, in MaterialPropertyEntity c0) => {
                var blocks = new MaterialPropertyBlock[b0.Length];
                for (int i = 0; i < b0.Length; i++) {
                    var current = b0[i];
                    var block = new MaterialPropertyBlock();

                    if (EntityManager.HasComponent<SharedTexture>(current.TextureEntity)) {
                        var texture = EntityManager.GetComponentData<SharedTexture>(current.TextureEntity).Value;
                        block.SetTexture(ShaderIDConstants.MainTex, texture);
                    }

                    blocks[i] = block;
                }
                EntityManager.AddComponentData(c0.Canvas, new MaterialPropertyBatch { Value = blocks });
                // if (renderElements.HasComponent(c0.Canvas) && spans.HasComponent(c0.Canvas)) {
                //     var renders = renderElements[c0.Canvas];
                //     var span    = spans[c0.Canvas];

                //     var blocks = new MaterialPropertyBlock[span.Length];

                //     for (int i = 0; i < span.Length; i++) {
                //         var current = span[i].Value;
                //         var renderedEntity = renders[current.x].Value;

                //         var propertyBlock = new MaterialPropertyBlock();

                //         if (linkedTextures.HasComponent(renderedEntity)) {
                //             var textureEntity = linkedTextures[renderedEntity].Value;
                //             var texture = EntityManager.GetComponentData<SharedTexture>(textureEntity).Value;
                //             propertyBlock.SetTexture(ShaderIDConstants.MainTex, texture);
                //         }

                //         if (colors.HasComponent(renderedEntity)) {
                //             var color = colors[renderedEntity].Value;
                //             propertyBlock.SetColor(ShaderIDConstants.Color, color);
                //         }
                //         blocks[i] = propertyBlock;
                //     }

                //     EntityManager.AddComponentData(c0.Canvas, new MaterialPropertyBatch { Value = blocks });
                // }
                EntityManager.DestroyEntity(entity);
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}