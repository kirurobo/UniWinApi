using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HumanoidColliderBuilder : MonoBehaviour
{

    public Animator anim;
    public ColliderPrm colliderPrm;

    [Range(0.5f, 1.5f)]
    public float legSize = 1, armSize = 1, headSize = 1;

    public List<GameObject> colliderObj;

    // Start is called before the first frame update
    void Start()
    {
        if (anim == null) { anim = GetComponent<Animator>(); }
    }

#if UNITY_EDITOR
    [Header("Debug")]
    public bool isSet;
    public bool isDestroy;
    // Update is called once per frame
    void Update()
    {
        if (isSet)
        {
            ResetData();

            SetCollider();

            isSet = false;
        }
        if (isDestroy)
        {
            ResetData();
            isDestroy = false;
        }
    }
#endif

    public void ResetData()
    {
        foreach (GameObject tmp in colliderObj)
        {
            Destroy(tmp);
        }
        colliderObj.Clear();
    }

    public void SetCollider()
    {
        if (anim != null)
        {
            SetBone();
        }
    }

    private void SetBone()
    {
        HumanBone[] boneUp = new HumanBone[6];
        boneUp[0] = GetBoneData(anim, HumanBodyBones.Hips);
        boneUp[1] = GetBoneData(anim, HumanBodyBones.Spine);
        boneUp[2] = GetBoneData(anim, HumanBodyBones.Chest);
        boneUp[3] = GetBoneData(anim, HumanBodyBones.UpperChest);
        boneUp[4] = GetBoneData(anim, HumanBodyBones.Neck);
        boneUp[5] = GetBoneData(anim, HumanBodyBones.Head);

        HumanBone[] boneDownLeft = new HumanBone[4];
        boneDownLeft[0] = GetBoneData(anim, HumanBodyBones.LeftUpperLeg);
        boneDownLeft[1] = GetBoneData(anim, HumanBodyBones.LeftLowerLeg);
        boneDownLeft[2] = GetBoneData(anim, HumanBodyBones.LeftFoot);
        boneDownLeft[3] = GetBoneData(anim, HumanBodyBones.LeftToes);

        HumanBone[] boneDownRight = new HumanBone[4];
        boneDownRight[0] = GetBoneData(anim, HumanBodyBones.RightUpperLeg);
        boneDownRight[1] = GetBoneData(anim, HumanBodyBones.RightLowerLeg);
        boneDownRight[2] = GetBoneData(anim, HumanBodyBones.RightFoot);
        boneDownRight[3] = GetBoneData(anim, HumanBodyBones.RightToes);

        HumanBone[] boneUpLeft = new HumanBone[4];
        boneUpLeft[0] = GetBoneData(anim, HumanBodyBones.LeftShoulder);
        boneUpLeft[1] = GetBoneData(anim, HumanBodyBones.LeftUpperArm);
        boneUpLeft[2] = GetBoneData(anim, HumanBodyBones.LeftLowerArm);
        boneUpLeft[3] = GetBoneData(anim, HumanBodyBones.LeftHand);

        HumanBone[] boneUpRight = new HumanBone[4];
        boneUpRight[0] = GetBoneData(anim, HumanBodyBones.RightShoulder);
        boneUpRight[1] = GetBoneData(anim, HumanBodyBones.RightUpperArm);
        boneUpRight[2] = GetBoneData(anim, HumanBodyBones.RightLowerArm);
        boneUpRight[3] = GetBoneData(anim, HumanBodyBones.RightHand);

        //肩幅＆腰幅
        float[] referenceX = new float[2];
        referenceX[1] = (boneDownLeft[0].tras.position - boneDownRight[0].tras.position).magnitude * 1.5f;
        referenceX[0] = (boneUpLeft[1].tras.position - boneUpRight[1].tras.position).magnitude;

        //Body親から生成
        SetBody(referenceX, boneUp, boneDownLeft);

        //頭
        SetHead(referenceX, boneUp);

        //脚
        //左
        SetLeg(referenceX, boneDownLeft);
        SetLeg(referenceX, boneDownRight);

        //腕
        //左
        SetArm(referenceX, boneUpLeft);
        SetArm(referenceX, boneUpRight);

    }

    void SetBody(float[] referenceX, HumanBone[] humanBones, HumanBone[] leftDown)
    {
        int dis = 0;
        for (int m = humanBones.Length - 2; m >= 0; m--)
        {
            if (humanBones[m].tras != null)
            {
                Transform target = humanBones[m + 1].tras;

                int disTmp = 0;
                if (target == null)
                {
                    for (int i = m + 2; i < humanBones.Length; i++)
                    {
                        if (humanBones[i].tras != null)
                        {
                            target = humanBones[i].tras;
                            disTmp = i - m - 1;
                            break;
                        }
                    }
                }
                if (target != null)
                {
                    //ボーンがnullの場合親,子サイズ調整prm
                    float vecTmp = (float)disTmp / 2 + dis;
                    float height = (referenceX[1] + (referenceX[0] * 1.2f - referenceX[1]) * (m / 3)) * 1.2f;

                    Vector3 forwardVec = (target.position - humanBones[m].tras.position);
                    Vector3 offset = forwardVec * (1 + vecTmp / 2) / 2;
                    offset = new Vector3(-offset.y, -offset.z, 0);
                    float radius = (forwardVec / 2).magnitude * 1.2f;

                    if (m != 4)
                    {
                        if (m == 0)
                        {
                            radius = (humanBones[0].tras.position - leftDown[0].tras.position).magnitude;
                        }

                        if (height > radius * 2)
                        {
                            forwardVec = Vector3.right;
                            offset = new Vector3(offset.y, -offset.x, 0) * ((float)m / 3);
                        }
                        else
                        {
                            offset = new Vector3(0, 0, -offset.x) * ((float)(m + 1) / 3);
                        }
                    }
                    else
                    {
                        radius *= 1.2f;
                        height = radius;
                        offset = new Vector3(0, 0, -offset.x) * ((float)m / 3);
                    }
                    CreateCollider(humanBones[m].bone, humanBones[m].tras, forwardVec, offset, radius, height, true, colliderPrm.isTrigger, colliderPrm.body.layer, colliderPrm.body.tag, colliderPrm.material);
                }
                else
                {
                    Debug.Log(humanBones[m].bone + " null");
                }
                dis = disTmp;
            }
            else
            {
                Debug.Log(humanBones[m].bone + " null");
            }
        }
    }

    void SetHead(float[] referenceX, HumanBone[] humanBones)
    {

        //肩幅から推定
        float tmp = referenceX[0] * 1.2f * headSize;
        Vector3 forwardVec = Vector3.up * tmp;
        float height = forwardVec.magnitude;
        Vector3 offset = new Vector3(0, 0, height / 2 * 0.9f);
        float radius = height / 2.2f;
        CreateCollider(humanBones[5].bone, humanBones[5].tras, forwardVec, offset, radius, height, false, colliderPrm.isTrigger, colliderPrm.head.layer, colliderPrm.head.tag, colliderPrm.material);
    }

    void SetLeg(float[] referenceX, HumanBone[] humanBones)
    {
        //腰幅から脚の太さ推定
        float radius = referenceX[1] / 3 * legSize * 0.8f;

        for (int m = 0; m < humanBones.Length - 1; m++)
        {
            if (m == 0) radius *= 1.2f;
            if (humanBones[m + 1].tras != null)
            {
                Vector3 forwardVec = -(humanBones[m].tras.position - humanBones[m + 1].tras.position);
                if (m == 2)
                {
                    radius *= 0.8f;
                    forwardVec = forwardVec.normalized * radius * 3f;
                }
                float height = forwardVec.magnitude * 1.2f;
                Vector3 offset = new Vector3(0, 0, height / 2);
                CreateCollider(humanBones[m].bone, humanBones[m].tras, forwardVec, offset, radius, height, false, colliderPrm.isTrigger, colliderPrm.leg.layer, colliderPrm.leg.tag, colliderPrm.material);
            }
        }

    }

    void SetArm(float[] referenceX, HumanBone[] humanBones)
    {
        //腰幅から腕の太さ推定
        float radius = referenceX[1] / 3.5f * armSize * 0.8f;

        for (int m = 0; m < humanBones.Length; m++)
        {

            if (humanBones[m].tras != null)
            {
                Vector3 forwardVec = Vector3.zero; ;
                if (m == 3)
                {
                    forwardVec = -(humanBones[m - 1].tras.position - humanBones[m].tras.position);
                    forwardVec = forwardVec.normalized * radius * 2f;
                }
                else
                {
                    forwardVec = -(humanBones[m].tras.position - humanBones[m + 1].tras.position);
                }
                float height = forwardVec.magnitude * 1.2f;
                Vector3 offset = new Vector3(0, 0, height / 2);
                CreateCollider(humanBones[m].bone, humanBones[m].tras, forwardVec, offset, radius, height, false, colliderPrm.isTrigger, colliderPrm.arm.layer, colliderPrm.arm.tag, colliderPrm.material);
            }
        }
    }

    private HumanBone GetBoneData(Animator animTmp, HumanBodyBones bone)
    {
        HumanBone tmp = new HumanBone();
        tmp.bone = bone;
        tmp.tras = animTmp.GetBoneTransform(bone);
        return tmp;
    }

    private void CreateCollider(HumanBodyBones type, Transform tras, Vector3 forwardVec, Vector3 offset, float radius, float height, bool isbody, bool isTrigger = true, string layer = "Deffault", string tag = "Untagged", PhysicMaterial pm = null)
    {
        GameObject root = Instantiate((GameObject)Resources.Load("HumanoidCollider/root"), tras.position, Quaternion.LookRotation(forwardVec));
        root.name = type.ToString() + " Collider";
        root.tag = tag;
        root.layer = LayerMask.NameToLayer(layer);
        root.transform.parent = tras.transform;
        CapsuleCollider capsule = root.GetComponent<CapsuleCollider>();
        if (radius * 2 > height && isbody)
        {
            float tmp = radius * 2;
            radius = height / 2;
            height = tmp;
        }
        capsule.center = offset;
        capsule.radius = radius;
        capsule.height = height;
        capsule.isTrigger = isTrigger;
        if (pm != null) { capsule.material = pm; }
        colliderObj.Add(root);
    }

    [System.Serializable]
    public struct ColliderPrm
    {
        public PhysicMaterial material;
        public bool isTrigger;
        public TagLayer head;
        public TagLayer body;
        public TagLayer arm;
        public TagLayer leg;
    }

    [System.Serializable]
    public class TagLayer
    {
        public string layer = "Default";
        public string tag = "Untagged";
    }

    public struct HumanBone
    {
        public Transform tras;
        public HumanBodyBones bone;
    }
}
