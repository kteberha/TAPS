using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAnimation : MonoBehaviour
{
    //9 animations
    [SerializeField] Texture2D[] annoyedFaces, blinkerFaces, flyingFaces, freakoutFaces, sadFaces, shockedFaces, smileFaces, sorryFaces, turnoffFaces;
    [SerializeField] Texture2D[] selectedAnimation;

    Texture2D currentFace;
    [SerializeField] Material faceMaterial;
    int faceFrameIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        selectedAnimation = flyingFaces;//set the default animation;
        currentFace = selectedAnimation[0];
        InvokeRepeating("AnimateFace", 0f, .5f);
    }

    public void AnimateFace()
    {
        if(faceFrameIndex < selectedAnimation.Length)
        {
            print(faceFrameIndex);
            faceMaterial.SetTexture("_Texture2D_FaceTexture", currentFace);//set the face texture to the adjusted current face
            if(faceFrameIndex + 1 < selectedAnimation.Length)//check array bounds
                currentFace = selectedAnimation[faceFrameIndex + 1];//set the current face to the frame ahead
            else
                faceFrameIndex = 0;//reset the face index to 0 when it has reached the end of the array
            faceFrameIndex++;//increase the int to get the next frame
        }
    }

    public void SetFreakOutFace()
    {
        selectedAnimation = freakoutFaces;
    }

    public void SetIdleFlyingFace()
    {
        selectedAnimation = flyingFaces;
    }


}
