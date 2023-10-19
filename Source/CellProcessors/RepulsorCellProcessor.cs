using Modding;
using Modding.PublicInterfaces.Cells;
using System.Threading;

namespace Indev2
{
    public class RepulsorCellProcessor : TickedCellStepper
    {
        public RepulsorCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Repulsor Cell";
        public override int CellType => 33;
        public override string CellSpriteIndex => "Repulsor";

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetCells())
            {
                if (ct.IsCancellationRequested)
                    return;
                foreach (var direction in Direction.All)
                {
                    var target = cell.Transform.Position + direction.AsVector2Int;
                    var targetCell = _cellGrid.GetCell(target);

                    if (targetCell == null)
                        continue;
                    if (targetCell.Value.Instance.Type == 20)
                        continue;
                    _cellGrid.PushCell(targetCell.Value, direction, 1);
                    continue;
                }
            }
        }

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if (force == -1)
            {
                if (!_cellGrid.InBounds(cell.Transform.Position + direction.AsVector2Int))
                    return false;
                return true;
            }
            if (force <= 1)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);

            if (targetCell is null)
            {
                _cellGrid.MoveCell(cell, target);
                return true;
            }

            if (!_cellGrid.PushCell(targetCell.Value, direction, force))
                return false;

            _cellGrid.MoveCell(cell, target);
            return true;
        }

        public override void Clear()
        {
        }

        public override void OnCellInit(ref BasicCell cell)
        {
        }
    }
}