using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selector : Observer
{
    public RectTransform selectionBox;
    public LayerMask unitLayerMask;

    private GameController _gameController;
    
    [SerializeField] private Camera cam;
    
    [SerializeField] private List<Unit> units;
    
    private List<Unit> _selectedUnits;

    private Vector2 _startPos;

    protected override void Start()
    {
        base.Start();
        _gameController = (GameController) subject;
        _selectedUnits = new List<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameController.GetState() != GameState.Running) return;

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.transform.gameObject.layer)
                {
                    case 9: //Wall
                        _selectedUnits.ForEach(unit => unit.SetTarget(hit.point));
                        break;
                    case 10: //Enemy
                        _selectedUnits.ForEach(unit => unit.SetTarget(hit.transform));
                        break;
                }
            }
            
            _startPos = Input.mousePosition;
            _selectedUnits.Clear();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }
            
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in from unit in units let screenPos = cam.WorldToScreenPoint(unit.transform.position) where screenPos.x > min.x && 
            screenPos.x < max.x &&
            screenPos.y > min.y &&
            screenPos.y < max.y select unit)
        {
            _selectedUnits.Add(unit);
            unit.SetSelected(true);
        }

        foreach (var unit in units.Where(unit => !_selectedUnits.Contains(unit)))
            unit.SetSelected(false);
    }

    private void UpdateSelectionBox(Vector2 currPos)
    {
        if(!selectionBox.gameObject.activeSelf)
            selectionBox.gameObject.SetActive(true);

        float width = currPos.x - _startPos.x;
        float height = currPos.y - _startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = _startPos + new Vector2(width / 2, height / 2);
    }
}
