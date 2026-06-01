using System;
using System.Collections.Generic;
using System.Linq;

public class HandResult : IComparable<HandResult>
{
    public HandRank HandRank { get; }
    // 外部から変更されないよう、読み取り専用リスト（または配列）として定義
    public IReadOnlyList<int> Tiebreakers { get; }

    public HandResult(HandRank rank, IEnumerable<int> tiebreakers)
    {
        this.HandRank = rank;
        // 渡された時点で配列として確定（固定）させる
        this.Tiebreakers = tiebreakers.ToArray();
    }

    public int CompareTo(HandResult other)
    {
        // 1. まず役の強さを比較する
        int compHandRank = this.HandRank.CompareTo(other.HandRank);
        if (compHandRank != 0) return compHandRank;

        // 2. 役が同じ場合、タイブレーカーを先頭から順に比較する
        // ※同じ役同士であればタイブレーカーの数（要素数）は同じになる前提
        for (int i = 0; i < this.Tiebreakers.Count; i++)
        {
            int compResult = this.Tiebreakers[i].CompareTo(other.Tiebreakers[i]);
            if (compResult != 0) return compResult; // 勝敗がついた時点で返す
        }

        // 3. 全てのタイブレーカーが同じなら引き分け
        return 0;
    }
}