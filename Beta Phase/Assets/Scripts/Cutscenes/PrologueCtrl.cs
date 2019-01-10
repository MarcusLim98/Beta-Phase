using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueCtrl : MonoBehaviour {

    public UiBehaviour ui;

    void EndPrologue()
    {
        //StartCoroutine("PrologueComplete");
        Time.timeScale = 0.25f;
        ui.LoadScene("Outside_Warehouse");
    }

    IEnumerator PrologueComplete()
    {
        yield return new WaitForSeconds(1);
        ui.LoadScene("Outside_Warehouse");
    }

}
