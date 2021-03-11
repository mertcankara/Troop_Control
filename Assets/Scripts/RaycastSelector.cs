using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastSelector : Observer
{
    [SerializeField] private Projector projector;
    private GameController _gameController;
    private List<Unit> _selectedUnits;
    private Collider[] _selection; 
    private Box _box;
    private bool _shift;
    [SerializeField] private Camera cam;
    private int _layerMask;
    private Transform _selectedTarget;
    private RaycastHit _hit;

    [SerializeField] private Text defaultNumberText;
    private int _defaultNumber;
    
    [SerializeField] private Text runnerNumberText;
    private int _runnerNumber;

    [SerializeField] private UnitParameterSetter unitHelper;

    [SerializeField] private Button selectAll;
    [SerializeField] private Button selectNone;
    
    private int Layer
    {
        get => _layerMask;
        set => _layerMask = 1 << value;
    }
    
    protected override void Start()
    {
        base.Start();
        _box = new Box();
        _shift = false;
        _selectedTarget = null;
        _gameController = (GameController) subject;
        _selectedUnits = new List<Unit>();
        _runnerNumber = _defaultNumber = 0;

        selectAll.interactable = selectNone.interactable = false;
        
        selectAll.onClick.AddListener(delegate { Array.ForEach(unitHelper.GetUnits(), unit => UnitClicked(unit, false)); });
        selectNone.onClick.AddListener(Clear);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.GetState() != GameState.Running) return;

        _shift = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out _hit, 500f, ~LayerMask.GetMask("Cage", "Wall"))) return;
            Layer = _hit.transform.gameObject.layer;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                _box.StartPos = _hit.point;
        }
        
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out _hit, 500f, LayerMask.GetMask("Target", "Cage"))) return;
                    
                Layer = _hit.transform.gameObject.layer;

                if (Layer == LayerMask.GetMask("Target"))
                {
                    _selectedTarget = _hit.transform;
                    _selectedUnits.ForEach(unit => unit.SetTarget(_selectedTarget.transform));
                }
                else if (Layer == LayerMask.GetMask("Cage")) _selectedUnits.ForEach(unit => unit.SetTarget(_hit.point));
            }

            if (ReferenceEquals(_selectedTarget, null)) return;
            
            TargetClicked(_selectedTarget.transform);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _selectedTarget = null;
        }
        else if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!_shift) Clear();
                
                if (Layer == LayerMask.GetMask("Unit")) UnitClicked(_hit.transform.GetComponent<Unit>(), true);
                else if (Layer != LayerMask.GetMask("Ground")) return;
                
                projector.enabled = true;
            }
            
            _box.EndPos = _hit.point;
            
            projector.aspectRatio = _box.Size.x / _box.Size.z;
            projector.orthographicSize = _box.Size.z / 2;
            projector.transform.position = _box.Center;
        } else if (Input.GetMouseButtonUp(1))
        {
            projector.enabled = false;
            ReleaseSelectionBox();
        }
    }

    private void TargetClicked(Transform targetTransform)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        
        if (!Physics.Raycast(ray, out var hit, 500f, LayerMask.GetMask("Cage"))) return;

        Vector3 newPos = targetTransform.position;
        newPos.x = hit.point.x;
        newPos.z = hit.point.z;

        targetTransform.position = newPos;
    }
    
    private void UnitClicked(Unit unit, bool remove)
    {
        if (_selectedUnits.Contains(unit))
        {
            if (!remove) return;
            
            unit.SetSelected(false);
            _selectedUnits.Remove(unit);
                
            SetSelectedCounts(unit, i => i - 1);

            return;
        }
        
        unit.SetSelected(true);
        _selectedUnits.Add(unit);

        SetSelectedCounts(unit, i => i + 1);
    }

    private void SetSelectedCounts(Unit unit, Func<int, int> func)
    {
        switch (unit)
        {
            case SoliderUnit _:
                _defaultNumber = func(_defaultNumber);
                defaultNumberText.text = _defaultNumber.ToString();
                break;
            case RunnerUnit _:
                _runnerNumber = func(_runnerNumber);
                runnerNumberText.text = _runnerNumber.ToString();
                break;
        }
    }
    
    private void Clear()
    {
        _selectedUnits.ForEach(unit => unit.SetSelected(false));
        _selectedUnits.Clear();

        _defaultNumber = 0;
        defaultNumberText.text = "0";
        
        _runnerNumber = 0;
        runnerNumberText.text = "0";
    }
    
    private void ReleaseSelectionBox()
    {
        _selection = Physics.OverlapBox(_box.Center, _box.Extends, Quaternion.identity, LayerMask.GetMask("Unit"));

        foreach (Collider obj in _selection)
        {
            Unit unit = obj.GetComponent<Unit>();
            UnitClicked(unit, false);
        }
    }
    
    public override void Notify(GameState state)
    {
        if (state == GameState.Running)
            selectAll.interactable = selectNone.interactable = true;
    }

    private class Box
    {
        public Vector3 StartPos { get; set; }

        public Vector3 EndPos { get; set; }

        public Vector3 Center
        {
            get
            {
                Vector3 center = StartPos + (EndPos - StartPos) / 2;
                center.y = 10f;
                return center;
            }
        }

        public Vector3 Size => new Vector3(Mathf.Abs(EndPos.x - StartPos.x), (EndPos - StartPos).magnitude, Mathf.Abs(EndPos.z - StartPos.z));

        public Vector3 Extends => Size / 2;
    }
}
