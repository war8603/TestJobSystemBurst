using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [SerializeField]
    WaveGeneratorUpdate _updateGen;

    [SerializeField]
    WaveGeneratorJobs _jobGen;

    [SerializeField]
    WaveGeneratorBurst _burstGen;

    [SerializeField]
    List<Image> _btnImages;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = -1;
#elif UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = 120;   
#endif        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickBtn(int genIndex)
    {
        for(int i = 0; i < _btnImages.Count; i++)
        {
            if (genIndex == i)
            {
                _btnImages[i].color = Color.white;
            }
            else
            {
                _btnImages[i].color = Color.gray;
            }
        }
        switch(genIndex)
        {
            case 0:
                _jobGen.OnStart(false);
                _burstGen.OnStart(false);

                _updateGen.OnStart(true);
                break;
            case 1:
                _updateGen.OnStart(false);
                _burstGen.OnStart(false);

                _jobGen.OnStart(true);
                break;
            case 2:
                _updateGen.OnStart(false);
                _jobGen.OnStart(false);
                
                _burstGen.OnStart(true);
                break;
            default:
                break;
        }
    }
}
