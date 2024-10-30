using System.Text;

const char playerLine = '_';
const char player = 'V';
const char sand = 'X';

object locker = new();
bool isRunning = true;
bool drop = false;

int consoleWindowWidth = Console.WindowWidth;
int consoleWindowHeight = (int)Math.Round( Console.WindowHeight * 0.90 );
int playerLocation = consoleWindowWidth / 2;

char[,] field = new char[consoleWindowHeight, consoleWindowWidth];
char[,] playerBar = new char[2, consoleWindowWidth];

void DropSand()
{
    int start = 1;
    int sandX = playerLocation;
    field[start, sandX] = sand;

    while ( true )
    {
        start++;
        if ( field[start, sandX] == sand )
        {
            int randomX = Random.Shared.Next( 0, 2 );
            switch ( randomX )
            {
                case < 1 when sandX < consoleWindowWidth - 1 && field[start, sandX + 1] != sand:
                    field[start - 1, sandX] = ' ';
                    ++sandX;
                    break;
                case < 1 when sandX > 0 && field[start, sandX - 1] != sand:
                    field[start - 1, sandX] = ' ';
                    --sandX;
                    break;
                case < 2 when sandX > 0 && field[start, sandX - 1] != sand:
                    field[start - 1, sandX] = ' ';
                    --sandX;
                    break;
                case < 2 when sandX < consoleWindowWidth - 1 && field[start, sandX + 1] != sand:
                    field[start - 1, sandX] = ' ';
                    ++sandX;
                    break;
            }
        }

        if ( field[start, sandX] != sand && start < consoleWindowHeight - 1 )
        {
            field[start - 1, sandX] = ' ';
            field[start, sandX] = sand;
            PrintField();
            continue;
        }

        break;
    }
}

void AllocateField()
{
    for ( int i = 0; i < consoleWindowHeight; i++ )
    {
        for ( int j = 0; j < consoleWindowWidth; j++ )
        {
            field[i, j] = ' ';
        }
    }
}

void AllocatePlayerBar()
{
    for ( int i = 0; i < consoleWindowWidth; i++ )
    {
        playerBar[0, i] = playerLine;
        playerBar[1, i] = ' ';
    }
}

void PrintField()
{
    StringBuilder sb = new();

    for ( int i = 0; i < 2; i++ )
    {
        for ( int j = 0; j < consoleWindowWidth; j++ )
        {
            if ( j == playerLocation && i == 1)
            {
                sb.Append( player );
                continue;
            }
            sb.Append( playerBar[i, j] );
        }

        sb.AppendLine();
    }

    for ( int i = 0; i < consoleWindowHeight; i++ )
    {
        for ( int j = 0; j < consoleWindowWidth; j++ )
        {
            sb.Append( field[i, j] );
        }

        sb.AppendLine();
    }

    Thread.Sleep( 1 );
    Console.SetCursorPosition(0, 0);
    Console.Write(sb.ToString());
}

void HandleMovement()
{
    while ( isRunning )
    {
        char key = Console.ReadKey( true ).KeyChar;
        lock ( locker )
        {
            switch ( key )
            {
                case 'l' when playerLocation < consoleWindowWidth - 1:
                    playerLocation++;
                    break;
                case 'h' when playerLocation > 0:
                    playerLocation--;
                    break;
                case 'r':
                    drop = !drop;
                    break;
                case 'q':
                    isRunning = false;
                    break;
            }
        }

        Thread.Sleep( 10 );
    }
}

Thread inputThread = new(HandleMovement);
inputThread.Start();

Console.CursorVisible = false;
AllocatePlayerBar();
AllocateField();
while ( isRunning )
{
    Thread.Sleep( 10 );
    if ( drop )
    {
        DropSand();
        continue;
    }

    PrintField();
}

inputThread.Join();