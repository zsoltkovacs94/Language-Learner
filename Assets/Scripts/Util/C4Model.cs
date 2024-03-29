using System.Diagnostics;
using System.Text;
using UnityEngine;

public class C4Model
{
    private char[,] state = new char[6, 7];
    private bool p1Turn = true;
    private char winner;
    private bool putLock = false;
    private string lastPut;
    public C4Model() 
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                state[i, j] = '0';
            }
        }
    }
    public int Put(int i)
    {
        if (putLock) return -1;
        if (i > 6 || i < 0)
        {
            return -1;
        }
        if (state[0, i] != '0')
        {
            return -1;
        }
        for (int j = 5; j >= 0; j--)
        {
            if (state[j,i] == '0')
            {
                lastPut = j.ToString() + ',' + i.ToString();
                if (p1Turn)
                {
                    state[j, i] = '1';
                    p1Turn = !p1Turn;
                    if(CheckWin(j, i))
                    {
                        winner = '1';
                        putLock = true;
                        return 1;
                    }
                    return 0;
                }
                else
                {
                    state[j, i] = '2';
                    p1Turn = !p1Turn;
                    if (CheckWin(j, i))
                    {
                        winner = '2';
                        putLock = true;
                        return 1;
                    }
                    return 0;
                }
            }
        }
        return -1;
    }
    public char GetWinner()
    {
        return winner;
    }
    private bool CheckWin(int i, int j)
    {
        char player = state[i, j];
        int inRow = 1;
        bool lLock = false;
        bool rLock = false;
        for (int k = 1; k < 4; k++)
        {
            if ((j + k) < 7 && !lLock)
            {
                if (state[i,j + k] == player)
                {
                    inRow++;
                }
                else
                {
                    lLock = true;
                }
            }
            if((j - k) >= 0 && !rLock)
            {
                if (state[i, j - k] == player)
                {
                    inRow++;
                }
                else
                {
                    rLock = true;
                }
            }
        }
        if (inRow >= 4)
        {
            return true;
        }
        else inRow = 1;
        bool uLock = false;
        bool dLock = false;
        for (int k = 1; k < 4; k++)
        {
            if ((i + k) < 6 && !dLock)
            {
                if (state[i + k, j] == player)
                {
                    inRow++;
                }
                else
                {
                    dLock = true;
                }
            }
            if ((i - k) >= 0 && !uLock)
            {
                if (state[i - k, j] == player)
                {
                    inRow++;
                }
                else
                {
                   uLock = true;
                }
            }
        }
        if (inRow >= 4)
        {
            return true;
        }
        else inRow = 1;
        bool uLLock = false;
        bool dRLock = false;
        for (int k = 1; k < 4; k++)
        {
            if ((i + k) < 6 && (j + k) < 7 && !dRLock)
            {
                if (state[i + k, j + k] == player)
                {
                    inRow++;
                }
                else
                {
                    dRLock = true;
                }
            }
            if ((i - k) >= 0 && (j - k) >= 0 && !uLLock)
            {
                if (state[i - k, j - k] == player)
                {
                    inRow++;
                }
                else
                {
                    uLLock = true;
                }
            }
        }
        if (inRow >= 4)
        {
            return true;
        }
        else inRow = 1;
        bool uRLock = false;
        bool dLLock = false;
        for (int k = 1; k < 4; k++)
        {
            if ((i - k) >= 0 && (j + k) < 7 && !uRLock)
            {
                if (state[i - k, j + k] == player)
                {
                    inRow++;
                }
                else
                {
                    uRLock = true;
                }
            }
            if ((i + k) < 6 && (j - k) >= 0 && !dLLock)
            {
                if (state[i + k, j - k] == player)
                {
                    inRow++;
                }
                else
                {
                    dLLock = true;
                }
            }
        }
        if (inRow >= 4)
        {
            return true;
        }
        return false;
    }
    public char GetTurn()
    {
        if (p1Turn) return '1';
        else return '2';
    }
    public char GetStateAt(int i, int j)
    {
        return state[i, j];
    }
    public string GetLastPut()
    {
        return lastPut;
    }
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                sb.Append(state[i, j]).Append(',');
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
