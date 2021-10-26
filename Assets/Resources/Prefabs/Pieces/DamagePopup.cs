using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class DamagePopup : MonoBehaviour
{
    [Header("Popup speed and live time")]
    public float jumpSpeed;
    public float awayVector;
    public float disappearTime;
    public float disappearSpeed;
    private float disappearTimer;
    private TextMeshPro textMesh;

    /// <summary>
    /// Creates a damage pop up at given locations with damage amount
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damageAmmount"></param>
    /// <returns></returns>
    public static DamagePopup Create(Vector3 position, int damageAmmount, float xAwayVector)
    {
        GameObject damagePopupTransform = GameAssets.Instance.GetObject(GameAssets.Instance.pfDamagePopup);// Instantiate(GameAssets.Instance.pfDamagePopup, Vector3.zero, Quaternion.identity);
        damagePopupTransform.transform.position = position;
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmmount, xAwayVector);
        return damagePopup;
    }

    /// <summary>
    /// Creates a damage pop up at given locations with damage amount
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damageAmmount"></param>
    /// <returns></returns>
    public static DamagePopup Create(Vector3 position, int damageAmmount)
    {
        return Create(position, damageAmmount, 0);
    }

    public void Awake()
    {
        textMesh = this.GetComponent<TextMeshPro>();
        disappearTimer = 0;
    }

    public void Update()
    {
        //this.transform.position += new Vector3(awayVector, moveSpeed) * Time.deltaTime;
        disappearTimer += Time.deltaTime;

        if (disappearTimer >= disappearTime)
        {
            textMesh.alpha -= disappearSpeed * Time.deltaTime;

            if (textMesh.alpha <= 0)
                Destroy(this.gameObject);
        }

    }

    public void Setup(int damageAmount, float xAwayVector)
    {
        textMesh.SetText(damageAmount.ToString());
        awayVector = xAwayVector;
        this.GetComponent<Rigidbody2D>().velocity = new Vector3(awayVector, jumpSpeed);
    }
}
