using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;

namespace VRMLoader
{
    public class VRMPreviewUI : MonoBehaviour {
        [Serializable]
        struct TextFields
        {
            [SerializeField, Header("Info")]
            Text m_textModelTitle;
            [SerializeField]
            Text m_textModelVersion;
            [SerializeField]
            Text m_textModelAuthor;
            [SerializeField]
            Text m_textModelContact;
            [SerializeField]
            Text m_textModelReference;
            [SerializeField]
            RawImage m_thumbnail;

            [SerializeField, Header("CharacterPermission")]
            Text m_textPermissionAllowed;
            [SerializeField]
            RawImage m_iconPermissionAllowed;
            [SerializeField]
            Text m_textPermissionViolent;
            [SerializeField]
            RawImage m_iconPermissionViolent;
            [SerializeField]
            Text m_textPermissionSexual;
            [SerializeField]
            RawImage m_iconPermissionSexual;
            [SerializeField]
            Text m_textPermissionCommercial;
            [SerializeField]
            RawImage m_iconPermissionCommercial;
            [SerializeField]
            Text m_textPermissionOther;

            [SerializeField, Header("DistributionLicense")]
            Text m_textDistributionLicense;
            [SerializeField]
            Text m_textDistributionOther;

            [SerializeField, Header("Icon Textures")]
            List<Texture2D> m_textureAllowedUser;
            [SerializeField]
            List<Texture2D> m_textureViolentUssage;
            [SerializeField]
            List<Texture2D> m_textureSexualUssage;
            [SerializeField]
            List<Texture2D> m_textureCommercialUssage;

            public void Start()
            {
                m_textModelTitle.text = "";
                m_textModelVersion.text = "";
                m_textModelAuthor.text = "";
                m_textModelContact.text = "";
                m_textModelReference.text = "";

                m_textPermissionAllowed.text = "";
                m_textPermissionViolent.text = "";
                m_textPermissionSexual.text = "";
                m_textPermissionCommercial.text = "";
                m_textPermissionOther.text = "";

                m_textDistributionLicense.text = "";
                m_textDistributionOther.text = "";
            }

            public void UpdateMeta(VRMMetaObject meta)
            {
                m_textModelTitle.text = meta.Title;
                m_textModelVersion.text = meta.Version;
                m_textModelAuthor.text = meta.Author;
                m_textModelContact.text = meta.ContactInformation;
                m_textModelReference.text = meta.Reference;

                m_textPermissionAllowed.text = m_textAllowedUser[(int) meta.AllowedUser];
                m_iconPermissionAllowed.texture = m_textureAllowedUser[(int) meta.AllowedUser];
                m_textPermissionViolent.text = m_textUsage[(int) meta.ViolentUssage];
                m_iconPermissionViolent.texture = m_textureViolentUssage[(int) meta.ViolentUssage];
                m_textPermissionSexual.text = m_textUsage[(int) meta.SexualUssage];
                m_iconPermissionSexual.texture = m_textureSexualUssage[(int) meta.SexualUssage];
                m_textPermissionCommercial.text = m_textUsage[(int) meta.CommercialUssage];
                m_iconPermissionCommercial.texture = m_textureCommercialUssage[(int) meta.CommercialUssage];
                m_textPermissionOther.text = meta.OtherPermissionUrl;

                m_textDistributionLicense.text = m_textLicenseType[(int) meta.LicenseType];
                m_textDistributionOther.text = meta.OtherLicenseUrl;

                if(meta.Thumbnail)
                    m_thumbnail.texture = meta.Thumbnail;
            }
        }
        [SerializeField]
        TextFields m_texts;

        static string[] m_textAllowedUser = {"作者のみ", "許可された人限定", "全員に許可"};
        static string[] m_textUsage = {"不許可", "許可"};
        static string[] m_textLicenseType = {
            "再配布禁止", 
            "著作権放棄(CC0)", 
            "表示 (CC BY 4.0)", 
            "表示 非営利 (CC BY-NC 4.0)",
            "表示 継承 (CC BY-SA 4.0)",
            "表示 非営利 継承 (CC BY-NC-SA 4.0)",
            "表示 改変禁止 (CC BY-ND 4.0)",
            "表示 非営利 改変禁止 (CC BY-NC-ND 4.0)",
            "その他" 
        };
        
        public Button m_ok, m_cancel;

        public void SetSelectionText(VRMPreviewLocale.Selections selections)
        {
            m_textAllowedUser = selections.PermissionAct;
            m_textUsage = selections.PermissionUsage;
            m_textLicenseType = selections.LicenseType;
        }
        public void setMeta(VRMMetaObject meta)
        {
            m_texts.UpdateMeta(meta);
        }

        public void setLoadable(bool loadable)
        {
            m_ok.interactable = loadable;
        }

        public void destroyMe()
        {
            Destroy(gameObject);
        }
    }
}
