using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour {

    public float viewAngle = 50;
    public float rotateSpeed = 1;
    public float attackDistance = 3;
    public float moveSpeed = 2;
    public float timeInterval = 1;
    public float[] attackArray;
    public GameObject bossBulletPrefab;
    public int hp = 1000;
    public string guid;
    public int targetRoleId = -1;//表示这个敌人要攻击的目标
    private GameObject targetGo;//表示要攻击的目标的游戏物体

    private float timer = 0;
    private Transform player;
    private bool isAttacking = false;
    private GameObject attack01GameObject;
    private GameObject attack02GameObject;
    private Transform attack03Pos;
    private Renderer renderer;


    private Vector3 lastPosition = Vector3.zero;
    private Vector3 lastEulerAngles = Vector3.zero;

    private bool lastStand = false;
    private bool lastAttack01 = false;
    private bool lastAttack02 = false;
    private bool lastAttack03 = false;
    private bool lastDie = false;
    private bool lastHit = false;
    private bool lastWalk = false;
    private BossController bossController;
	// Use this for initialization
    void Start() {
        targetGo = GameController.Instance.GetPlayerByRoleID(targetRoleId);
        //player = TranscriptManager._instance.player.transform;
        player = targetGo.transform;
        TranscriptManager._instance.AddEnemy(this.gameObject);
        attack01GameObject = transform.Find("attack01").gameObject;
        attack02GameObject = transform.Find("attack02").gameObject;
        attack03Pos = transform.Find("attack03Pos");
	    renderer = transform.Find("Object01").GetComponent<Renderer>();
        BossHPProgressBar.Instance.Show(hp);

        bossController = GetComponent<BossController>();
        bossController.OnSyncBossAnimation += this.OnSyncBossAnimation;
        if (GameController.Instance.battleType == BattleType.Team && GameController.Instance.isMaster)
        {
            InvokeRepeating("CheckPositionAndRotation", 0, 1 / 30f);
            InvokeRepeating("CheckAnimation",0,1f/30);
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    renderer.material.color = Color.Lerp(renderer.material.color, Color.white, Time.deltaTime);
        if(GameController.Instance.battleType==BattleType.Person||(GameController.Instance.battleType==BattleType.Team&&GameController.Instance.isMaster)){
	        if (hp <= 0) return;
	        if (GetComponent<Animation>().IsPlaying("hit")) return;
            if (isAttacking == true) return;
            Vector3 playerPos = player.position;
            playerPos.y = transform.position.y;//保证夹角不受到y轴的影响
            float angle = Vector3.Angle(playerPos - transform.position, transform.forward); 

            if (angle < viewAngle / 2) {
                //在攻击视野之内
                float distance = Vector3.Distance(player.position, transform.position);
                if (distance < attackDistance) {
                    //进行攻击
                    if (isAttacking == false) {
                        GetComponent<Animation>().CrossFade("stand");
                        timer += Time.deltaTime;
                        if (timer > timeInterval) {
                            timer = 0;
                            Attack();
                        }
                    }
                } else {
                    //进行追击
                    GetComponent<Animation>().CrossFade("walk");
                    GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
                }
            } else {
                //在攻击视野之外 进行转向
                GetComponent<Animation>().CrossFade("walk");
                Quaternion targetRotation = Quaternion.LookRotation(playerPos - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
            }
        }
	}
    private int attackIndex = 0;
    void Attack() {
        isAttacking = true;
        attackIndex++;
        if (attackIndex == 4) attackIndex = 1;
        GetComponent<Animation>().CrossFade("attack0" + attackIndex);
        //if (attackIndex == 1) {
        //    animation.CrossFade("attack01");
        //} else if (attackIndex == 2) {
        //    animation.CrossFade("attack02");
        //} else if (attackIndex == 3) {
        //    animation.CrossFade("attack03");
        //}
    }
    void BackToStand() {
        isAttacking = false;
    }

    void PlayAttack01Effect() {
        attack01GameObject.SetActive(true);

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < attackDistance) {
            player.SendMessage("TakeDamage", attackArray[0], SendMessageOptions.DontRequireReceiver);
        }
    }

    void PlayAttack02Effect() {
        attack02GameObject.SetActive(true);

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance < attackDistance) {
            player.SendMessage("TakeDamage", attackArray[1], SendMessageOptions.DontRequireReceiver);
        }
    }

    void PlayAttack03Effect() {
        GameObject go = GameObject.Instantiate(bossBulletPrefab, attack03Pos.position, attack03Pos.rotation) as GameObject;
        BossBullet bb=  go.GetComponent<BossBullet>();
        bb.Damage = attackArray[2];
    }

    //收到攻击调用这个方法
    // 0,收到多少伤害
    // 1,后退的距离
    // 2,浮空的高度
    void TakeDamage(string args) {
        if (hp <= 0) return;
        isAttacking = false;
        Combo._instance.ComboPlus();
        string[] proArray = args.Split(',');
        //减去伤害值
        int damage = int.Parse(proArray[0]);
        hp -= damage;
        BossHPProgressBar.Instance.UpdateShow(hp);
        // 更新Boss的血条
        //        受到攻击的动画
        if(Random.Range(0,10)==9)
            GetComponent<Animation>().Play("hit");
        //浮空和后退
        float backDistance = float.Parse(proArray[1]);
        float jumpHeight = float.Parse(proArray[2]);
        if(Random.Range(0,10)>7){
            iTween.MoveBy(this.gameObject,
                transform.InverseTransformDirection(TranscriptManager._instance.player.transform.forward) * backDistance
                + Vector3.up * jumpHeight,
                0.3f);
        }
        //出血效果 
        renderer.material.color = Color.red;
        
        if (hp <= 0) {
            Dead();
        }
    }

    void Dead()
    {
        GetComponent<Animation>().Play("die");
        BossHPProgressBar.Instance.Hide();
        TranscriptManager._instance.RemoveEnemy(this.gameObject);
        GameController.Instance.OnBossDie();
    }
    void CheckPositionAndRotation() {
        Vector3 position = transform.position;
        Vector3 eulerAngles = transform.eulerAngles;
        if (position.x != lastPosition.x || position.y != lastPosition.y || position.z != lastPosition.z ||
            eulerAngles.x != lastEulerAngles.x || eulerAngles.y != lastEulerAngles.y ||
            eulerAngles.z != lastEulerAngles.z) {
            TranscriptManager._instance.AddBossToSync(this);
            lastPosition = position;
            lastEulerAngles = eulerAngles;
        }
    }

    void CheckAnimation()
    {
        if (lastStand != GetComponent<Animation>().IsPlaying("stand") || lastAttack01 != GetComponent<Animation>().IsPlaying("attack01") ||
            lastAttack02 != GetComponent<Animation>().IsPlaying("attack02") || lastAttack03 != GetComponent<Animation>().IsPlaying("attack03") ||
            lastDie != GetComponent<Animation>().IsPlaying("die") || lastHit != GetComponent<Animation>().IsPlaying("hit") ||
            lastWalk != GetComponent<Animation>().IsPlaying("walk"))
        {
            //同步
            bossController.SyncBossAnimation(new BossAnimationModel(){attack01 = GetComponent<Animation>().IsPlaying("attack01"),attack02 = GetComponent<Animation>().IsPlaying("attack02"),attack03 = GetComponent<Animation>().IsPlaying("attack03"),die = GetComponent<Animation>().IsPlaying("die"),hit = GetComponent<Animation>().IsPlaying("hit"),stand = GetComponent<Animation>().IsPlaying("stand"),walk = GetComponent<Animation>().IsPlaying("walk")});

            lastStand = GetComponent<Animation>().IsPlaying("stand");
            lastAttack01 = GetComponent<Animation>().IsPlaying("attack01");
            lastAttack02 = GetComponent<Animation>().IsPlaying("attack02");
            lastAttack03 = GetComponent<Animation>().IsPlaying("attack03");
            lastDie = GetComponent<Animation>().IsPlaying("die");
            lastHit = GetComponent<Animation>().IsPlaying("hit");
            lastWalk = GetComponent<Animation>().IsPlaying("walk");
        }
    }

    public void OnSyncBossAnimation(BossAnimationModel model)
    {
        if (model.stand)
        {
            GetComponent<Animation>().Play("stand");
        }
        if (model.attack01)
        {
            GetComponent<Animation>().Play("attack01");
        }
        if (model.attack02)
        {
            GetComponent<Animation>().Play("attack02");
        }
        if (model.attack03)
        {
            GetComponent<Animation>().Play("attack03");
        }
        if (model.die)
        {
            GetComponent<Animation>().Play("die");
        }
        if (model.hit)
        {
            GetComponent<Animation>().Play("hit");
        }
        if (model.walk)
        {
            GetComponent<Animation>().Play("walk");
        }
    }
}
