using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InGameScene : MonoBehaviour
{
    [Header("테스트")]
    [SerializeField] private bool isTest = false;

    [Header("UI")]
     public GameObject _loadingSceneUI;

    [Header("Manager")]
    [SerializeField] private WorldManager _worldManager;

    [SerializeField] private ShopManager _shopManager;
    [SerializeField] private BoidsManager _boidsManager;
    [SerializeField] private Resource _resource;

    [SerializeField] private int worldCount = 0;

    [SerializeField] private BaseAI _robot;
    [SerializeField] private BaseAI _enemy;
    [SerializeField] private Flock _flock;
    public TrainEngine engine;

    // 석환이 형 isStart가 true일 때만 player가 작동할 수 있게 해줘~~
    public bool isStart { get; private set; } = false;

    private bool _isEnding = false;

    private void Awake()
    {
        // 게임 로드
        FileManager.LoadGame();

        // 로딩
        _loadingSceneUI.SetActive(true);
        isStart = false;

        // 로딩 시작
        LoadingFirstGame(isTest, 
            () =>
            {
                _loadingSceneUI.SetActive(false);
                RePositionAsync(
                    () =>
                    {
                        Instantiate(_robot, Vector3.up * 0.5f, Quaternion.identity).SetHome(FindObjectOfType<Resource>());
                        Instantiate(_enemy, Vector3.up * 0.5f + Vector3.right, Quaternion.identity).SetHome(FindObjectOfType<Resource>());

                        for (int i = 0; i < 5; i++)
                        {
                            Vector3 pos = Vector3.up * 0.5f + Random.insideUnitSphere * 7;
                            Flock flock = Instantiate(_flock);
                            flock.transform.position = new Vector3(pos.x, 0.5f, pos.z);
                        }
                        _boidsManager.Init();


                    }).Forget();

                _shopManager.StartTrainMove();
            }).Forget();
    }


    /// <summary>
    /// 역 도착하면 실행될 메소드
    /// </summary>
    public void ArriveStation()
    {
        // 만약 마지막 역이라면 엔딩
        if(_isEnding)
        {
            Debug.Log("엔딩입니다~~~~");
            StartCoroutine(engine.ClearAnim());

        }
        else
        {
            // 플레이어 손에 든거 내려놓기
            FindObjectOfType<PlayerController>().PutDownItem();

            Helper helper = FindObjectOfType<Helper>();
            helper.arr();

            // 바리케이드 올려주기
            Transform barricadeparent = GameObject.Find("BarricadeParent").transform;
            barricadeparent.position += Vector3.up * 20f;

            // 바리케이드 중간 없애주기
            for (int i = 0; i < _worldManager.betweenBarricadeTransform.Count; i++)
            {
                Destroy(_worldManager.betweenBarricadeTransform[i].gameObject);
            }
            _worldManager.betweenBarricadeTransform.Clear();


            // 볼트 하나 추가 해주고

            // 조금있다가 상점보여주기

            _shopManager.ShopOn();
        }
    }

    /// <summary>
    /// 역 떠나면 실행될 메소드
    /// </summary>
    public void LeaveStation()
    {

        // 올린 바리케이드 내려주기
        Transform barricadeparent = GameObject.Find("BarricadeParent").transform;
        barricadeparent.position -= Vector3.up * 20f;

        Helper helper = FindObjectOfType<Helper>();
        RespawnTool();

        // 나가기 게이지 100되면 실행될 메소드

        // 새로운 역 세팅
        _shopManager.currentStation = _worldManager.stations[2].transform;

        // 상점 종료되고
        _shopManager.ShopOff();



        // 맵 재위치 시켜주기
        UnitaskInvoke(1.5f, () => { RePositionAsync().Forget(); helper.arr(); }).Forget();

        // 맵은 2개 밖에 없으니까 한번 역을 떠나면 엔딩준비 완료
        _isEnding = true;
    }

    private void RespawnTool()
    {
        // 현재 역 근처에 도구들 세팅
        Transform blockParent = _shopManager.currentStation.parent;
        List<MyItem> tools = new List<MyItem>();
        tools.Add(FindObjectOfType<MyBucketItem>());
        MyNonStackableItem[] items = FindObjectsOfType<MyNonStackableItem>();

        for (int i = 0; i < items.Length; i++)
        {
            tools.Add(items[i]);
        }

        for (int i = 0; i < tools.Count; i++)
        {
            tools[i].transform.SetParent(BFS(blockParent));
            tools[i].transform.localPosition = Vector3.up * 0.5f;
            tools[i].transform.localRotation = Quaternion.identity;
        }
    }

    private Transform BFS(Transform startTransform)
    {
        Transform currentTransform = startTransform;
        while (currentTransform.childCount != 0)
        {
            InfiniteLoopDetector.Run();

            if (Physics.Raycast(currentTransform.position, Vector3.back, out RaycastHit hit, 1f, 1 << LayerMask.NameToLayer("Block")))
            {
                currentTransform = hit.transform;
            }
        }
        return currentTransform;
    }


    private async UniTaskVoid RePositionAsync(System.Action onFinishedAsyncEvent = null)
    {
        await _worldManager.RePositionAsync(worldCount++);
        onFinishedAsyncEvent?.Invoke();
    }

    private async UniTaskVoid LoadingFirstGame(bool isTest, System.Action onCreateNextMapAsyncEvent = null)
    {
        // 맵 생성
        await _worldManager.GenerateWorld(isTest);
        onCreateNextMapAsyncEvent?.Invoke();
    }

    private async UniTaskVoid UnitaskInvoke(float time, System.Action action)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(time));

        action?.Invoke();
    }
}
