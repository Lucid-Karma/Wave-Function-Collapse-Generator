using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformController : MonoBehaviour
{
    //private Vector3 baslangicYon;
    //private Vector3 sonYon;
    //private float egimMiktari = 5f;

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        baslangicYon = Input.mousePosition;
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        sonYon = Input.mousePosition;
    //        EðimHesapla();
    //    }

    //    if (Input.touchCount > 0)
    //    {
    //        if (Input.GetTouch(0).phase == TouchPhase.Began)
    //        {
    //            baslangicYon = Input.GetTouch(0).position;
    //        }

    //        if (Input.GetTouch(0).phase == TouchPhase.Moved)
    //        {
    //            sonYon = Input.GetTouch(0).position;
    //            EðimHesapla();
    //        }
    //    }
    //}

    //void EðimHesapla()
    //{
    //    Vector3 yonFarki = sonYon - baslangicYon;
    //    float egimAcisi = Mathf.Atan2(yonFarki.y, yonFarki.x) * Mathf.Rad2Deg;
    //    transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, egimAcisi * egimMiktari * Time.deltaTime));
    //}

    //transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
}
