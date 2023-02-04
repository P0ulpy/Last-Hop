using UnityEngine;

public class RacineHorizontale : MonoBehaviour
{
     public enum Direction {Right, Left};
    [SerializeField] private GameObject racineVerticale;
    [SerializeField] private SpriteRenderer _rootSprite;
    [SerializeField] private GameObject _Mask;

    public float speed = 6;
    public float hauteur = -1;
    public float durationComeBack = 2f;
    
    
    private bool returning = false;
    private Vector3 lastLocation;
    private Vector3 firstLocation;
    private bool Stop = false;
    private float time;
    private Vector2 screensBounds;
    private float startPositionX;
    private bool canMoveNow = false;
    //temp
   
    // Update is called once per frame

    private void Awake()
    {
        screensBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        time = 0;
    }

    void Update()
    {
        if (returning) Reset();
        if (!Stop && canMoveNow) Move();
    }
    
    private void Move()
    {
        _Mask.transform.localPosition = new Vector3(_Mask.transform.localPosition.x + ( speed * Time.deltaTime), 0, 0);
        
        if (Mathf.Abs(_Mask.transform.position.x) > Mathf.Abs(startPositionX + screensBounds.x))
        {
            StopAimingThenShoot();
        }
    }
    public void StopAimingThenShoot()
    {
        lastLocation = _Mask.transform.localPosition;
        Stop = true;
        var racineVerticalevar = Instantiate(racineVerticale, new Vector3(_Mask.transform.position.x ,hauteur,0) , Quaternion.identity)
            .GetComponent<RacineVerticale>();
        
        racineVerticalevar.OnRetract += () =>
        {
            Destroy(racineVerticalevar.gameObject);
            returning = true;
        };

    }

    private void Reset()
    {
        _Mask.transform.localPosition = Vector3.Lerp(lastLocation, firstLocation, time / durationComeBack);
        time += Time.deltaTime;
        if (time >= durationComeBack) Destroy(this.gameObject);
    }

    public void StartAiming(Direction myDir)
    {
        if (myDir == Direction.Right)
        {
            canMoveNow = true;
        }
        else if(myDir == Direction.Left)
        {
            transform.Rotate(new Vector3(0,0,180));
            canMoveNow = true;
        }
        this.transform.position = new Vector3(0,hauteur,0); 
        startPositionX = _Mask.transform.localPosition.x + transform.position.x;
        firstLocation = new Vector3(startPositionX, hauteur, 0);
    }
}
