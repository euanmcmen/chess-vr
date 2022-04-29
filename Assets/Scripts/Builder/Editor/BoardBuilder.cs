using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    //[SerializeField]
    //private GameObject lightGraveBoardOrigin;

    //[SerializeField]
    //private GameObject darkGraveBoardOrigin;

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

    [SerializeField]
    private GameObject graveTilePrefab;

    [SerializeField]
    private GameObject graveTileContainerPrefab;

    private static readonly Dictionary<int, int> indexArrayTilePositionMultiplierMap = new()
    {
        { -2, -7},
        { -1, -6},

        { 1, -4 },
        { 2, -3 },
        { 3, -2 },
        { 4, -1 },
        { 5, 1 },
        { 6, 2 },
        { 7, 3 },
        { 8, 4 },

        { 10, 6 },
        { 11, 7 },
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

    [MenuItem("Chess/Board Builder")]
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindow<BoardBuilder>(false, "Board Builder");

        var width = 400;
        var height = 600;
        var x = (Screen.currentResolution.width - width) / 2;
        var y = (Screen.currentResolution.height - height) / 2;

        window.position = new Rect(x, y, width, height);

        window.Show();
    }

    private void OnGUI()
    {
        EditorHelper.DrawEditorControlsInGroup("Origins", false, () => {
            boardOrigin = EditorHelper.DrawTypedObjectField("Board Origin", boardOrigin, allowSceneObjects: true);
            //lightGraveBoardOrigin = EditorHelper.DrawTypedObjectField("Light Grave Origin", lightGraveBoardOrigin, allowSceneObjects: true);
            //darkGraveBoardOrigin = EditorHelper.DrawTypedObjectField("Dark Grave Origin", darkGraveBoardOrigin, allowSceneObjects: true);
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
            graveTilePrefab = EditorHelper.DrawTypedObjectField("Grave Tile Prefab", graveTilePrefab);
            graveTileContainerPrefab = EditorHelper.DrawTypedObjectField("Grave Tile Container Prefab", graveTileContainerPrefab);
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

            if (GUILayout.Button("Generate Grave Boards"))
            {
                GenerateGraveBoards();
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

    private void GenerateGraveBoards()
    {
        int[] lightColumnArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        int[] lightRowArray = new int[] { -1 , - 2 };
        int[] darkColumnArray = new int[] { 8, 7, 6, 5, 4, 3, 2, 1 };
        int[] darkRowArray = new int[] { 10, 11 };

        float tileLength = graveTilePrefab.transform.localScale.x;

        var lightGraveContainer = PrefabUtility.InstantiatePrefab(graveTileContainerPrefab, boardOrigin.transform) as GameObject;
        lightGraveContainer.GetComponent<GraveBoardApiScript>().Team = ChessPieceTeam.Light;

        foreach (var i in lightRowArray)
        {
            float tilePositionZ = GetTilePositionFromRowColumnIndex(i, tileLength);

            foreach (var j in lightColumnArray)
            {
                float tilePositionX = GetTilePositionFromRowColumnIndex(j, tileLength);
                var tile = PrefabUtility.InstantiatePrefab(graveTilePrefab, lightGraveContainer.transform) as GameObject;
                tile.transform.position = new Vector3(tilePositionX, lightGraveContainer.transform.position.y, tilePositionZ);
            }
        }

        var darkGraveContainer = PrefabUtility.InstantiatePrefab(graveTileContainerPrefab, boardOrigin.transform) as GameObject;
        darkGraveContainer.GetComponent<GraveBoardApiScript>().Team = ChessPieceTeam.Dark;

        foreach (var i in darkRowArray)
        {
            float tilePositionZ = GetTilePositionFromRowColumnIndex(i, tileLength);

            foreach (var j in darkColumnArray)
            {
                float tilePositionX = GetTilePositionFromRowColumnIndex(j, tileLength);
                var tile = PrefabUtility.InstantiatePrefab(graveTilePrefab, darkGraveContainer.transform) as GameObject;
                tile.transform.position = new Vector3(tilePositionX, darkGraveContainer.transform.position.y, tilePositionZ);
            }
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
