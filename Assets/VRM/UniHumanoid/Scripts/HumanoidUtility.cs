using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniHumanoid
{
    public static class Extensions
    {
        public static IEnumerable<Transform> GetChildren(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                yield return child;
            }
        }

        public static IEnumerable<Transform> Traverse(this Transform parent)
        {
            yield return parent;

            foreach (Transform child in parent)
            {
                foreach (Transform descendant in Traverse(child))
                {
                    yield return descendant;
                }
            }
        }

        public static SkeletonBone ToSkeletonBone(this Transform t)
        {
            var sb = new SkeletonBone();
            sb.name = t.name;
            sb.position = t.localPosition;
            sb.rotation = t.localRotation;
            sb.scale = t.localScale;
            return sb;
        }
    }


    public static class HumanoidUtility
    {
        static Transform GetLeftLeg(Transform[] joints)
        {
            Transform t = joints[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (joints[i].transform.position.x < t.position.x)
                {
                    t = joints[i];
                }
            }
            return t;
        }

        static Transform GetRightLeg(Transform[] joints)
        {
            Transform t = joints[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (joints[i].transform.position.x > t.position.x)
                {
                    t = joints[i];
                }
            }
            return t;
        }

        static Transform GetSpine(Transform[] joints)
        {
            Transform t = joints[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (joints[i].transform.position.y > t.position.y)
                {
                    t = joints[i];
                }
            }
            return t;
        }

        static Transform GetChest(Transform spine)
        {
            var current = spine;
            while (current != null)
            {
                if (current.childCount >= 3)
                {
                    return current;
                }

                current = current.GetChild(0);
            }
            return null;
        }

        static Transform GetLeftArm(Transform chest, Transform[] joints, Vector3 leftDir)
        {
            var values = joints.Select(x => Vector3.Dot((x.position - chest.position).normalized, leftDir)).ToArray();

            var current = joints[0];
            var value = values[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (values[i] > value)
                {
                    value = values[i];
                    current = joints[i];
                }
            }
            return current;
        }

        static Transform GetRightArm(Transform chest, Transform[] joints, Vector3 rightDir)
        {
            var values = joints.Select(x => Vector3.Dot((x.position - chest.position).normalized, rightDir)).ToArray();

            var current = joints[0];
            var value = values[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (values[i] > value)
                {
                    value = values[i];
                    current = joints[i];
                }
            }
            return current;
        }

        static Transform GetNeck(Transform[] joints)
        {
            Transform t = joints[0];
            for (int i = 1; i < joints.Length; ++i)
            {
                if (joints[i].transform.position.y > t.position.y)
                {
                    t = joints[i];
                }
            }
            return t;
        }

        public static IEnumerable<KeyValuePair<HumanBodyBones, Transform>> TraverseSkeleton(Transform root, Transform[] joints)
        {
            var rootJoints = joints.Where(x => !joints.Contains(x.parent)).ToArray();

            if (rootJoints.Length != 1)
            {
                yield break;
            }

            var hips = rootJoints[0];
            if (hips.childCount < 3)
            {
                yield break;
            }

            var hipsChildren = hips.GetChildren().ToArray();

            var spine = GetSpine(hipsChildren);

            var chest = GetChest(spine);
            var chestChildren = chest.GetChildren().ToArray();

            var neck = GetNeck(chestChildren);
            Transform head = null;
            if (neck.childCount == 0)
            {
                head = neck;
                neck = null;
            }
            else
            {
                head = neck.GetChild(0);
            }

            yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Hips, hips);

            //
            // left leg
            //
            var leftLeg = GetLeftLeg(hipsChildren).Traverse().Where(x => !x.name.ToLower().Contains("buttock")).ToArray();
            {
                if (leftLeg.Length == 3)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperLeg, leftLeg[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerLeg, leftLeg[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftFoot, leftLeg[2]);
                }
                else if (leftLeg.Length == 4)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperLeg, leftLeg[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerLeg, leftLeg[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftFoot, leftLeg[2]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftToes, leftLeg[3]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            //
            // right leg
            //
            var rightLeg = GetRightLeg(hipsChildren).Traverse().Where(x => !x.name.ToLower().Contains("buttock")).ToArray();
            {
                if (rightLeg.Length == 3)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperLeg, rightLeg[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerLeg, rightLeg[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightFoot, rightLeg[2]);
                }
                else if (rightLeg.Length == 4)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperLeg, rightLeg[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerLeg, rightLeg[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightFoot, rightLeg[2]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightToes, rightLeg[3]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Spine, spine);
            if (chest != spine)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Chest, chest);
            }
            if (neck != null)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Neck, neck);
            }
            yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Head, head);

            var rightDir = (rightLeg[0].position - leftLeg[0].position).normalized;

            //
            // left Arm
            //
            {
                var leftArm = GetLeftArm(chest, chestChildren, -rightDir).Traverse().ToArray();

                if (leftArm.Length == 2)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[1]);
                }
                else if (leftArm.Length == 3)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftHand, leftArm[2]);
                }
                else if (leftArm.Length >= 4)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftShoulder, leftArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[2]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftHand, leftArm[3]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            //
            // right Arm
            //
            {
                var rightArm = GetRightArm(chest, chestChildren, rightDir).Traverse().ToArray();

                if (rightArm.Length == 2)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[1]);
                }
                else if (rightArm.Length == 3)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightHand, rightArm[2]);
                }
                else if (rightArm.Length >= 4)
                {
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightShoulder, rightArm[0]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[1]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[2]);
                    yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightHand, rightArm[3]);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
