using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererFillAmount : MonoBehaviour
{
    public enum FillType
    {
        Vertical,
        Horizontal
    }

    [SerializeField, HideInInspector]
    private SpriteRenderer _spriteRenderer;

    [SerializeField, Range(0f, 1f)] private float _fillAmount = 1f;
    [SerializeField] private bool reverce = false;
    [SerializeField] private FillType type;

    public float FillAmount {
        get {
            return _fillAmount;
        }
        set {
            _fillAmount = Mathf.Clamp01(value);
            if (_material != null) {
                _material.SetFloat(PropertyId, _fillAmount);
            }
        }
    }
    public bool Reverce {
        get { return reverce; }
        set {
            reverce = value;
            if (_material != null) {
                _material.SetFloat("_Reverse", reverce ? 1 : 0);
            }
        }
    }
    public FillType Type {
        get { return type; }
        set {
            type = value;
            SetMaterial();
        }
    }

    private void OnValidate()
    {
        SetMaterial();
    }

    private static readonly int PropertyId = Shader.PropertyToID("_FillAmount");

    private Material _material;

    private void Reset()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        SetMaterial();
    }

    private void SetMaterial()
    {
        string str;

        if (type == FillType.Horizontal) {
            str = "Hidden/SpriteRendererFillAmountHorizontal";
        }
        else {
            str = "Hidden/SpriteRendererFillAmountVertical";
        }

        var shader = Shader.Find(str);
        if (shader == null) {
            Debug.LogWarning("Shader Not Found");
            return;
        }
        _material = new Material(shader);
        _material.SetFloat("_Reverse", reverce ? 1 : 0);

        //Material ÇÃç∑Çµë÷Ç¶
        _spriteRenderer.material = _material;
        FillAmount = _fillAmount;
    }
}