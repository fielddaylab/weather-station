using FieldDay;
using FieldDay.Components;
using FieldDay.SharedState;
using FieldDay.Systems;

using UnityEngine;
using UnityEngine.XR;

namespace WeatherStation {
    public class PlayerHandRig : SharedStateComponent {
		#region Inspector
        public PlayerHand LeftHand;
        public PlayerHand RightHand;
		public Animator LeftHandAnimator;
		public Animator RightHandAnimator;
		public float InputChangeRate = 20.0f;
		#endregion // Inspector
		
        private int AnimLayerIndexPointRight = -1;
        private int AnimLayerIndexPointLeft = -1;
		
		private int AnimParamIndexFlex = -1;
		private int AnimParamIndexPose = -1;
		
		private float LeftPB = 0f;
		private float RightPB = 0f;
		
		private void Awake() {
			if(LeftHandAnimator != null) {
				AnimLayerIndexPointLeft = LeftHandAnimator.GetLayerIndex("Point Layer");
			}
			
			if(RightHandAnimator != null) {
				AnimLayerIndexPointRight = RightHandAnimator.GetLayerIndex("Point Layer");
			}
			
			AnimParamIndexFlex = Animator.StringToHash("Flex");
			AnimParamIndexPose = Animator.StringToHash("Pose");		
		}
		
		public void UpdateStates() {
			
			VRInputState data = Game.SharedState.Get<VRInputState>();
			
			LeftHandAnimator.SetInteger(AnimParamIndexPose, 0);
			RightHandAnimator.SetInteger(AnimParamIndexPose, 0);
			
			float LeftGrip = data.LeftHand.Axis.Grip;
			float RightGrip = data.RightHand.Axis.Grip;
			
			LeftGrip = Mathf.Clamp(LeftGrip, 0.0f, 0.5f);
			RightGrip = Mathf.Clamp(RightGrip, 0.0f, 0.5f);
			
			LeftHandAnimator.SetFloat(AnimParamIndexFlex, LeftGrip);
			RightHandAnimator.SetFloat(AnimParamIndexFlex, RightGrip);
			
			//LeftPB = InputValueRateChange(data.LeftHand.Holding(VRControllerButtons.Trigger), LeftPB);
			//RightPB = InputValueRateChange(data.RightHand.Holding(VRControllerButtons.Trigger), RightPB);
			
			//LeftPB = Mathf.Clamp(LeftPB, 0.0f, 1.0f);
			//RightPB = Mathf.Clamp(RightPB, 0.0f, 1.0f);
			
			LeftHandAnimator.SetLayerWeight(AnimLayerIndexPointLeft, 1.0f-LeftGrip*2f);
			RightHandAnimator.SetLayerWeight(AnimLayerIndexPointRight, 1.0f-RightGrip*2f);
			
			LeftHandAnimator.SetFloat("Pinch", 0f);
			RightHandAnimator.SetFloat("Pinch", 0f);
		}
		
		private float InputValueRateChange(bool isDown, float value) {
            float rateDelta = Time.deltaTime * InputChangeRate;
            float sign = isDown ? 1.0f : -1.0f;
            return Mathf.Clamp01(value + rateDelta * sign);
        }
    }
}