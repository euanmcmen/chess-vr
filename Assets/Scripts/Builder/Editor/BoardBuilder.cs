using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ChessPiecePrefabPair
{
    [SerializeField]
    public ChessPieceType ChessPiece;

    [SerializeField]
    public GameObject Prefab;
}

public class BoardBuilder : EditorWindow
{
    [SerializeField]
    private Material boardLightMaterial;

    [SerializeField]
    private Material boardDarkMaterial;

    [SerializeField]
    private Material pieceLightMaterial;

    [SerializeField]
    private Material pieceDarkMaterial;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject boardOrigin;

    [SerializeField]
    private GameObject pawnPrefab;

    [SerializeField]
    private GameObject bishopPrefab;

    [SerializeField]
    private GameObject knightPrefab;

    [SerializeField]
    private GameObject rookPrefab;

    [SerializeField]
    private GameObject queenPrefab;

    [SerializeField]
    private GameObject kingPrefab;

    private static readonly Dictionary<int, int> indexArrayTilePositionMultiplierMap = new()
    {
        { 1, -4 },
        { 2, -3 },
        { 3, -2 },
        { 4, -1 },
        { 5, 1 },
        { 6, 2 },
        { 7, 3 },
        { 8, 4 }
    };

    private static readonly Dictionary<int, char> indexArrayRowNameMap = new()
    {
        { 1, 'a' },
        { 2, 'b' },
        { 3, 'c' },
        { 4, 'd' },
        { 5, 'e' },
        { 6, 'f' },
        { 7, 'g' },
        { 8, 'h' }
    };

    [MenuItem("Chess/Build Board")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<BoardBuilder>(false, "Board Builder");

        var width = 400;
        var height = 300;
        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        window.position = new Rect(x, y, width, height);

        window.Show();
    }

    private void OnGUI()
    {
        EditorHelper.DrawEditorControlsInGroup("Origin", false, () => {
            boardOrigin = EditorHelper.DrawTypedObjectField("Origin", boardOrigin, allowSceneObjects: true);
        });

        EditorHelper.DrawEditorControlsInGroup("Board Materials", false, () => {
            boardLightMaterial = EditorHelper.DrawTypedObjectField("Light Material", boardLightMaterial);
            boardDarkMaterial = EditorHelper.DrawTypedObjectField("Dark Material", boardDarkMaterial);
        });

        EditorHelper.DrawEditorControlsInGroup("Piece Materials", false, () => {
            pieceLightMaterial = EditorHelper.DrawTypedObjectField("Light Material", pieceLightMaterial);
            pieceDarkMaterial = EditorHelper.DrawTypedObjectField("Dark Material", pieceDarkMaterial);
        });

        EditorHelper.DrawEditorControlsInGroup("Board Prefabs", false, () => {
            tilePrefab = EditorHelper.DrawTypedObjectField("Tile Prefab", tilePrefab);
        });

        EditorHelper.DrawEditorControlsInGroup("Piece Prefabs", false, () => {
            pawnPrefab = EditorHelper.DrawTypedObjectField("Pawn Prefab", pawnPrefab);
            bishopPrefab = EditorHelper.DrawTypedObjectField("Bishop Prefab", bishopPrefab);
            knightPrefab = EditorHelper.DrawTypedObjectField("Knight Prefab", knightPrefab);
            rookPrefab = EditorHelper.DrawTypedObjectField("Rook Prefab", rookPrefab);
            queenPrefab = EditorHelper.DrawTypedObjectField("Queen Prefab", queenPrefab);
            kingPrefab = EditorHelper.DrawTypedObjectField("King Prefab", kingPrefab);
        });

        EditorHelper.DrawEditorControlsInGroup("Operations", false, () =>
        {
            if (GUILayout.Button("Generate Board"))
            {
                GenerateBoard();
            }

            if (GUILayout.Button("Place Pieces"))
            {
                PlacePieces();
            }
        });
    }

    private void GenerateBoard()
    {
        int[] indexArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        float tileLength = tilePrefab.transform.localScale.x;

        bool useDarkMaterial = true;

        foreach (var i in indexArray)
        {
            float tilePositionZ = GetTilePositionFromRowColumnIndex(i, tileLength);

            foreach (var j in indexArray)
            {
                float tilePositionX = GetTilePositionFromRowColumnIndex(j, tileLength);

                var tile = PrefabUtility.InstantiatePrefab(tilePrefab, boardOrigin.transform) as GameObject;

                tile.transform.position = new Vector3(tilePositionX, boardOrigin.transform.position.y, tilePositionZ);

                tile.GetComponent<Renderer>().material = useDarkMaterial ? boardDarkMaterial : boardLightMaterial;

                tile.name = $"{indexArrayRowNameMap[j]}{i}";

                useDarkMaterial = !useDarkMaterial;
            }

            useDarkMaterial = !useDarkMaterial;
        }
    }

    private void PlacePieces()
    {
        PlaceTeamPieces(1, pieceLightMaterial, ChessPieceTeam.Light);
        PlacePawnsOnRow(2, pieceLightMaterial, ChessPieceTeam.Light);
        PlacePawnsOnRow(7, pieceDarkMaterial, ChessPieceTeam.Dark);
        PlaceTeamPieces(8, pieceDarkMaterial, ChessPieceTeam.Dark);
    }

    private void PlacePawnsOnRow(int row, Material material, ChessPieceTeam team)
    {
        foreach (Transform tile in boardOrigin.transform)
        {
            if (tile.gameObject.name.EndsWith(row.ToString()))
            {
                PlacePiece(tile, pawnPrefab, material, team, ChessPieceType.Pawn);
            }
        }
    }

    private void PlaceTeamPieces(int row, Material material, ChessPieceTeam team)
    {
        PlaceStandardPieceOnPosition($"c{row}", bishopPrefab, material, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPosition($"f{row}", bishopPrefab, material, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPosition($"b{row}", knightPrefab, material, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPosition($"g{row}", knightPrefab, material, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPosition($"a{row}", rookPrefab, material, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPosition($"h{row}", rookPrefab, material, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPosition($"d{row}", queenPrefab, material, team, ChessPieceType.Queen);
        PlaceStandardPieceOnPosition($"e{row}", kingPrefab, material, team, ChessPieceType.King);
    }

    private void PlaceStandardPieceOnPosition(string notation, GameObject piecePrefab, Material material, ChessPieceTeam team, ChessPieceType type)
    {
        PlacePiece(boardOrigin.transform.Find(notation), piecePrefab, material, team, type);
    }

    private void PlacePiece(Transform tileTransform, GameObject piecePrefab, Material material, ChessPieceTeam team, ChessPieceType type)
    {
        var piece = PrefabUtility.InstantiatePrefab(piecePrefab, boardOrigin.transform) as GameObject;

        piece.transform.position = new Vector3(tileTransform.position.x, piece.transform.position.y, tileTransform.position.z);

        piece.GetComponent<Renderer>().material = material;

        var pieceScript = piece.GetComponent<PieceScript>();
        pieceScript.Team = team;
        pieceScript.Type = type;
        pieceScript.InitialPositionNotation = tileTransform.gameObject.name;
    }

    private float GetTilePositionFromRowColumnIndex(int index, float tileLength)
    {
        return (indexArrayTilePositionMultiplierMap[index] * tileLength) - (tileLength / 2 * Mathf.Sign(indexArrayTilePositionMultiplierMap[index]));
    }
}
