using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    GameObject sprite0_;
    GameObject spriteInstance0_;
    GameObject spriteInstance1_;
    GameObject spriteInstance2_;

    SpriteRenderer spriteRenderer_;

    int createNum_ = 0;
    float createTimer_ = 0.0f;
    bool useAfterImage_ = false;
    [SerializeField] float createTime_;
    [SerializeField] float fadeSpeed_;

    Vector3 createPosition_ = Vector3.zero;
    Vector3 createScale_ = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        sprite0_ = (GameObject)Resources.Load("Afterimage");

        spriteInstance0_ = Instantiate(sprite0_, new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
        spriteInstance1_ = Instantiate(sprite0_, new Vector3(2.0f, 2.0f, 0.0f), Quaternion.identity);
        spriteInstance2_ = Instantiate(sprite0_, new Vector3(4.0f, 2.0f, 0.0f), Quaternion.identity);

        SetColorAlpha(spriteInstance0_, 0.0f);
        SetColorAlpha(spriteInstance1_, 0.0f);
        SetColorAlpha(spriteInstance2_, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Žc‘œ‚ðŽg—p‚·‚é
        if (useAfterImage_)
        {
            if(createTimer_ <= 0.0f)
            {
                if(createNum_ == 0)
                {
                    SetColorAlpha(spriteInstance0_, 1.0f);

                    spriteInstance0_.transform.position = createPosition_;
                    spriteInstance0_.transform.localScale = createScale_;

                    ++createNum_;
                }
                else if(createNum_ == 1)
                {
                    SetColorAlpha(spriteInstance1_, 1.0f);

                    spriteInstance1_.transform.position = createPosition_;
                    spriteInstance1_.transform.localScale = createScale_;

                    ++createNum_;
                }
                else
                {
                    SetColorAlpha(spriteInstance2_, 1.0f);

                    spriteInstance2_.transform.position = createPosition_;
                    spriteInstance2_.transform.localScale = createScale_;

                    useAfterImage_ = false;
                }

                createTimer_ = createTime_;
            }

            createTimer_ -= Time.deltaTime;
        }

        FadeSprite(spriteInstance0_);
        FadeSprite(spriteInstance1_);
        FadeSprite(spriteInstance2_);
    }

    void SetColorAlpha(GameObject obj, float alpha)
    {
        spriteRenderer_ = obj.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer_.color;
        color.a = alpha;
        spriteRenderer_.color = color;
    }

    void FadeSprite(GameObject obj)
    {
        spriteRenderer_ = obj.GetComponent<SpriteRenderer>();
        Color color = spriteRenderer_.color;
        color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * fadeSpeed_);
        spriteRenderer_.color = color;
    }

    public void UseAfterImage()
    {
        useAfterImage_ = true;
        createNum_ = 0;
        createTimer_ = 0.0f;
    }

    public void UpdateTransform(Vector3 position, Vector3 scale)
    {
        createPosition_ = position;
        createScale_ = scale;
    }
}
