using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugViewer : MonoBehaviour {

    public SettlingRoomsCreator levelGenerator;

    private void Start()
    {
        //levelGenerator.GenerateLevel();
    }

    private void Update()
    {
        if (levelGenerator == null)
        {
            return;
        }

        foreach (DelaunayMSTRoom cell in levelGenerator.rooms)
        {
            Color color = Color.gray;
            if (cell.isMainRoom)
            {
                color = Color.red;
            }
            else if (cell.isPathRoom)
            {
                color = Color.magenta;
            }

            Vector3 bottomLeft = new Vector3(cell.x, 0, cell.y);
            Vector3 bottomRight = new Vector3(cell.x + cell.width, 0, cell.y);

            Vector3 topLeft = new Vector3(cell.x, 0, cell.y + cell.height);
            Vector3 topRight = new Vector3(cell.x + cell.width, 0, cell.y + cell.height);

            Debug.DrawLine(bottomLeft, bottomRight, color);
            Debug.DrawLine(bottomLeft, topLeft, color);
            Debug.DrawLine(topRight, topLeft, color);
            Debug.DrawLine(topRight, bottomRight, color);

            color = Color.green;

            foreach (Delaunay.Geo.LineSegment delaunayLine in levelGenerator.delaunayLines)
            {
                Vector3 lineStart = new Vector3(delaunayLine.p0.Value.x, 0, delaunayLine.p0.Value.y);
                Vector3 lineEnd = new Vector3(delaunayLine.p1.Value.x, 0, delaunayLine.p1.Value.y);

                Debug.DrawLine(lineStart, lineEnd, color);
            }

            color = Color.blue;

            foreach (Delaunay.Geo.LineSegment spanningTreeLine in levelGenerator.spanningTree)
            {
                Vector3 lineStart = new Vector3(spanningTreeLine.p0.Value.x, 0, spanningTreeLine.p0.Value.y);
                Vector3 lineEnd = new Vector3(spanningTreeLine.p1.Value.x, 0, spanningTreeLine.p1.Value.y);

                Debug.DrawLine(lineStart, lineEnd, color);
            }

            color = Color.yellow;

            foreach (RoomsConnection path in levelGenerator.paths)
            {
                foreach (Line blockPath in path.path)
                {
                    Vector3 lineStart = new Vector3(blockPath.start.x, 0, blockPath.start.y);
                    Vector3 lineEnd = new Vector3(blockPath.end.x, 0, blockPath.end.y);

                    Debug.DrawLine(lineStart, lineEnd, color);
                }
            }

            color = Color.white;
            float step = 2 * Mathf.PI / 20;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += step)
            {
                var x = transform.position.x + levelGenerator.settings.roomSpawnCircleRadius * Mathf.Cos(theta);
                var y = transform.position.y + levelGenerator.settings.roomSpawnCircleRadius * Mathf.Sin(theta);
                var xPrev = transform.position.x + levelGenerator.settings.roomSpawnCircleRadius * Mathf.Cos(theta - step);
                var yPrev = transform.position.y + levelGenerator.settings.roomSpawnCircleRadius * Mathf.Sin(theta - step);
                Debug.DrawLine(new Vector3(xPrev, 0, yPrev), new Vector3(x, 0, y), color);

                x = cell.x + 0.1f * Mathf.Cos(theta);
                y = cell.y + 0.1f * Mathf.Sin(theta);
                xPrev = cell.x + 0.1f * Mathf.Cos(theta - step);
                yPrev = cell.y + 0.1f * Mathf.Sin(theta - step);
                Debug.DrawLine(new Vector3(xPrev, 0, yPrev), new Vector3(x, 0, y), color);
            }

        }
    }
}
