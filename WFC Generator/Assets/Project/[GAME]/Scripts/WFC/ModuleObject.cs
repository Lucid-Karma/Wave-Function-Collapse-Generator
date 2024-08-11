using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class ModuleObject: NetworkBehaviour, IModuleObject
{
    public bool isChecked = false;
    public int Row;
    public int Column;

    public int north;
    public int south;
    public int east;
    public int west;

    int _north, _south, _east, _west;

    GameObject _cityPart;

    public bool isRotatable;


    void OnEnable()
    {
        if (gameObject.transform.childCount > 0)
           _cityPart = gameObject.transform.GetChild(0).gameObject;
        ActivateCity();

        RotateCells.OnGridCollapse.AddListener(() => isChecked = false);
        RotateCells.OnModulesRotate.AddListener(DeactivateCity);
        EventManager.OnLevelFinish.AddListener(ActivateCity);
    }
    void OnDisable()
    {
        RotateCells.OnGridCollapse.RemoveListener(() => isChecked = false);
        RotateCells.OnModulesRotate.RemoveListener(DeactivateCity);
        EventManager.OnLevelFinish.RemoveListener(ActivateCity);

        isChecked = false;

        // ModuleSO already updates these variables on OnEnable..!!!
        north = _north;
        south = _south;
        east = _east;
        west = _west;

        Row = 0;
        Column = 0;
    }

    public void UpdateMO_Angle(Transform moduleTransform)
    {
        _north = north;
        _south = south;
        _east = east;
        _west = west;

        north = _west;
        south = _east;
        east = _north;
        west = _south;
    }

    private void ActivateCity()
    {
        if (_cityPart != null)
        {
            _cityPart.SetActive(true);
        }
    }

    #region City Network Activation
    private void DeactivateCity()
    {
        if (_cityPart != null)
        {
            if(GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.SinglePlayer)
            {
                _cityPart.SetActive(false);
            }
            else
            {
                if (NetworkManager.IsHost)
                {
                    //_cityPart.SetActive(false);
                    DeactivateCityClientRpc();
                }
            }
            
        }
    }

    [ClientRpc]
    private void DeactivateCityClientRpc()
    {
        _cityPart.SetActive(false);
    }
    #endregion

    #region RotateProcess
    public void RotateModuleForDrawn(int randomIndex, int randomRotation)
    {
        transform.DORotate(new Vector3(0f, (float)randomRotation, 0f), 1f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutQuad);
        for (int i = 0; i < randomIndex + 1; i++)
        {
            UpdateMO_Angle(transform);
        }
    }
    public void RotateModule()
    {
        //Debug.Log($"RotateModule Role: IsHost={IsHost}");
       
        RotateCells.Instance.isRotating = true;

        transform.DORotate(new Vector3(0f, 90f, 0f), 0.4f, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Unset)
            .OnComplete(() => { RotateCells.Instance.isRotating = false;  UpdateMO_Angle(transform); 
                                                                    if (IsAuthority()) { EventManager.OnClick.Invoke(); } 
                EventManager.OnCollapseEnd.Invoke();
            });

        
        //modulePrefab.Rotate(Vector3.up, 90f);
    }
    [ServerRpc(RequireOwnership = false)]
    public void RotatePuzzlePieceServerRpc(ServerRpcParams rpcParams = default)
    {
        RotatePuzzlePieceClientRpc();
    }

    [ClientRpc]
    private void RotatePuzzlePieceClientRpc()
    {
        RotateModule();
    }

    private bool IsAuthority()
    {
        if(GameModeManager.Instance.CurrentGameMode == GameModeManager.GameMode.Multiplayer)
        {
            if (IsHost) return true;
            return false;
        }
        return true;
    }
    #endregion
}
