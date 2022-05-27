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

    [SerializeField]
    private GameObject graveTilePrefab;

    [SerializeField]
    private GameObject graveTileContainerPrefab;

    [SerializeField]
    private bool overridePiecePrefabMaterials;

    [SerializeField]
    private ChessPieceSetSO basePieceSet;

    [SerializeField]
    private ChessPieceSetSO lightPieceSet;

    [SerializeField]
    private ChessPieceSetSO darkPieceSet;

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
        });

        EditorHelper.DrawEditorControlsInGroup("Board Prefabs", false, () => {
            tilePrefab = EditorHelper.DrawTypedObjectField("Tile Prefab", tilePrefab);
            graveTilePrefab = EditorHelper.DrawTypedObjectField("Grave Tile Prefab", graveTilePrefab);
            graveTileContainerPrefab = EditorHelper.DrawTypedObjectField("Grave Tile Container Prefab", graveTileContainerPrefab);
        });

        EditorHelper.DrawEditorControlsInGroup("Board Materials", false, () => {
            boardLightMaterial = EditorHelper.DrawTypedObjectField("Light Material", boardLightMaterial);
            boardDarkMaterial = EditorHelper.DrawTypedObjectField("Dark Material", boardDarkMaterial);
        });

        EditorHelper.DrawEditorControlsInGroup("Piece Materials", false, () => {
            overridePiecePrefabMaterials = EditorGUILayout.Toggle("Override Piece Materials", overridePiecePrefabMaterials);
            if (overridePiecePrefabMaterials)
            {
                pieceLightMaterial = EditorHelper.DrawTypedObjectField("Light Material", pieceLightMaterial);
                pieceDarkMaterial = EditorHelper.DrawTypedObjectField("Dark Material", pieceDarkMaterial);
            }
        });

        EditorHelper.DrawEditorControlsInGroup("Piece Sets", false, () => {
            if (overridePiecePrefabMaterials)
            {
                basePieceSet = EditorHelper.DrawTypedObjectField("Set", basePieceSet);
            }
            else
            {
                lightPieceSet = EditorHelper.DrawTypedObjectField("Light Set", lightPieceSet);
                darkPieceSet = EditorHelper.DrawTypedObjectField("Dark Set", darkPieceSet);
            }
        });

        EditorGUILayout.Space();

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
                if (overridePiecePrefabMaterials)
                {
                    PlacePiecesAndApplyMaterials();
                }
                else
                {
                    PlacePieces(ChessPieceTeam.Light);
                    PlacePieces(ChessPieceTeam.Dark);
                }
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
        float tileLength = graveTilePrefab.transform.localScale.x;

        var lightGraveContainer = PrefabUtility.InstantiatePrefab(graveTileContainerPrefab, boardOrigin.transform) as GameObject;
        lightGraveContainer.GetComponent<GraveBoardScript>().Team = ChessPieceTeam.Light;

        int[] lightColumnArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        int[] lightRowArray = new int[] { -1, -2 };

        foreach (var i in lightRowArray)
        {
            float tilePositionZ = GetTilePositionFromRowColumnIndex(i, tileLength);

            foreach (var j in lightColumnArray)
            {
                float tilePositionX = GetTilePositionFromRowColumnIndex(j, tileLength);
                var tile = PrefabUtility.InstantiatePrefab(graveTilePrefab, lightGraveContainer.transform) as GameObject;
                tile.transform.position = new Vector3(tilePositionX, lightGraveContainer.transform.position.y, tilePositionZ);
                tile.name = $"{ChessPieceTeam.Light}Grave{i}{j}";
            }
        }

        var darkGraveContainer = PrefabUtility.InstantiatePrefab(graveTileContainerPrefab, boardOrigin.transform) as GameObject;
        darkGraveContainer.GetComponent<GraveBoardScript>().Team = ChessPieceTeam.Dark;

        int[] darkColumnArray = new int[] { 8, 7, 6, 5, 4, 3, 2, 1 };
        int[] darkRowArray = new int[] { 10, 11 };

        foreach (var i in darkRowArray)
        {
            float tilePositionZ = GetTilePositionFromRowColumnIndex(i, tileLength);

            foreach (var j in darkColumnArray)
            {
                float tilePositionX = GetTilePositionFromRowColumnIndex(j, tileLength);
                var tile = PrefabUtility.InstantiatePrefab(graveTilePrefab, darkGraveContainer.transform) as GameObject;
                tile.transform.position = new Vector3(tilePositionX, darkGraveContainer.transform.position.y, tilePositionZ);
                tile.name = $"{ChessPieceTeam.Dark}Grave{i}{j}";
            }
        }
    }

    private void PlacePiecesAndApplyMaterials()
    {
        if (!overridePiecePrefabMaterials)
            return;

        PlaceTeamPiecesAndApplyMaterials(1, pieceLightMaterial, ChessPieceTeam.Light);
        PlacePawnsOnRowAndApplyMaterials(2, pieceLightMaterial, ChessPieceTeam.Light);
        PlacePawnsOnRowAndApplyMaterials(7, pieceDarkMaterial, ChessPieceTeam.Dark);
        PlaceTeamPiecesAndApplyMaterials(8, pieceDarkMaterial, ChessPieceTeam.Dark);
    }

    private void PlacePieces(ChessPieceTeam team)
    {
        if (overridePiecePrefabMaterials)
            return;

        var (pawnRow, standardRow) = GetRowSpawnNumbersForTeam(team);

        PlacePawnsOnRow(pawnRow, team);
        PlaceTeamPieces(standardRow, team);
    }

    private void PlacePawnsOnRowAndApplyMaterials(int row, Material material, ChessPieceTeam team)
    {
        foreach (Transform tile in boardOrigin.transform)
        {
            if (tile.gameObject.name.EndsWith(row.ToString()))
            {
                PlacePieceAndApplyMaterial(tile, basePieceSet.PawnPrefab, material, team, ChessPieceType.Pawn);
            }
        }
    }

    private void PlacePawnsOnRow(int row, ChessPieceTeam team)
    {
        foreach (Transform tile in boardOrigin.transform)
        {
            if (tile.gameObject.name.EndsWith(row.ToString()))
            {
                PlacePiece(tile, GetSetForTeam(team).PawnPrefab, team, ChessPieceType.Pawn);
            }
        }
    }

    private void PlaceTeamPiecesAndApplyMaterials(int row, Material material, ChessPieceTeam team)
    {
        PlaceStandardPieceOnPositionAndApplyMaterials($"c{row}", basePieceSet.BishopPrefab, material, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPositionAndApplyMaterials($"f{row}", basePieceSet.BishopPrefab, material, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPositionAndApplyMaterials($"b{row}", basePieceSet.KnightPrefab, material, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPositionAndApplyMaterials($"g{row}", basePieceSet.KnightPrefab, material, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPositionAndApplyMaterials($"a{row}", basePieceSet.RookPrefab, material, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPositionAndApplyMaterials($"h{row}", basePieceSet.RookPrefab, material, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPositionAndApplyMaterials($"d{row}", basePieceSet.QueenPrefab, material, team, ChessPieceType.Queen);
        PlaceStandardPieceOnPositionAndApplyMaterials($"e{row}", basePieceSet.KingPrefab, material, team, ChessPieceType.King);
    }

    private void PlaceTeamPieces(int row, ChessPieceTeam team)
    {
        PlaceStandardPieceOnPosition($"c{row}", GetSetForTeam(team).BishopPrefab, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPosition($"f{row}", GetSetForTeam(team).BishopPrefab, team, ChessPieceType.Bishop);
        PlaceStandardPieceOnPosition($"b{row}", GetSetForTeam(team).KnightPrefab, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPosition($"g{row}", GetSetForTeam(team).KnightPrefab, team, ChessPieceType.Knight);
        PlaceStandardPieceOnPosition($"a{row}", GetSetForTeam(team).RookPrefab, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPosition($"h{row}", GetSetForTeam(team).RookPrefab, team, ChessPieceType.Rook);
        PlaceStandardPieceOnPosition($"d{row}", GetSetForTeam(team).QueenPrefab, team, ChessPieceType.Queen);
        PlaceStandardPieceOnPosition($"e{row}", GetSetForTeam(team).KingPrefab, team, ChessPieceType.King);
    }

    private void PlaceStandardPieceOnPositionAndApplyMaterials(string notation, GameObject piecePrefab, Material material, ChessPieceTeam team, ChessPieceType type)
    {
        PlacePieceAndApplyMaterial(boardOrigin.transform.Find(notation), piecePrefab, material, team, type);
    }

    private void PlaceStandardPieceOnPosition(string notation, GameObject piecePrefab,ChessPieceTeam team, ChessPieceType type)
    {
        PlacePiece(boardOrigin.transform.Find(notation), piecePrefab, team, type);
    }

    private void PlacePieceAndApplyMaterial(Transform tileTransform, GameObject piecePrefab, Material material, ChessPieceTeam team, ChessPieceType type)
    {
        PlacePiece(tileTransform, piecePrefab, team, type)
            .GetComponent<Renderer>().material = material;
    }

    private GameObject PlacePiece(Transform tileTransform, GameObject piecePrefab, ChessPieceTeam team, ChessPieceType type)
    {
        var piece = PrefabUtility.InstantiatePrefab(piecePrefab, boardOrigin.transform) as GameObject;

        piece.transform.position = new Vector3(tileTransform.position.x, piece.transform.position.y, tileTransform.position.z);

        var pieceScript = piece.GetComponent<PieceScript>();
        pieceScript.Team = team;
        pieceScript.Type = type;
        pieceScript.InitialPositionNotation = tileTransform.gameObject.name;

        piece.name = $"{piece.name} {tileTransform.gameObject.name[..1].ToUpper()}";

        return piece;
    }

    private float GetTilePositionFromRowColumnIndex(int index, float tileLength)
    {
        return (indexArrayTilePositionMultiplierMap[index] * tileLength) - (tileLength / 2 * Mathf.Sign(indexArrayTilePositionMultiplierMap[index]));
    }

    private ChessPieceSetSO GetSetForTeam(ChessPieceTeam chessPieceTeam)
    {
        return chessPieceTeam switch
        {
            ChessPieceTeam.Light => lightPieceSet,
            ChessPieceTeam.Dark => darkPieceSet,
            _ => throw new NotImplementedException(),
        };
    }

    private (int pawnRow, int standardRow) GetRowSpawnNumbersForTeam(ChessPieceTeam chessPieceTeam)
    {
        return chessPieceTeam switch
        {
            ChessPieceTeam.Light => (2, 1),
            ChessPieceTeam.Dark => (7, 8),
            _ => throw new NotImplementedException(),
        };
    }
}
