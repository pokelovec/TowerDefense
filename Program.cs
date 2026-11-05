using System.Data;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
enum Gamestate {Welcome, Battle, Pause, Upgrade, Death, Exit, HowToPlay}
public struct Bullet 
{
    public Rectangle Rect;
    public Vector2 Velocity;
}
public struct EnemyBullet
{
    public Rectangle Rect;
    public Vector2 Velocity;
    public float damage;
}
public struct Enemy
{
    public Rectangle Rect;
    public float enemyLvl;
    public float enemyHp;
    public float enemyDmg;
    public float enemySpd;
    public List<Vector2> path;
    public int NextPointIndex;
    public bool isHit;
    public float hitTimer;
    public float shootTimer;
}
class Program
{
    static void Main()
    {
        Raylib.InitWindow(1, 1, "emental");
        Raylib.ToggleFullscreen();
        Raylib.SetTargetFPS(60);
        int screenHeight = Raylib.GetScreenHeight();
        int screenWidth = Raylib.GetScreenWidth();
        Color DarkRed = new Color(139, 0, 0, 255);
        Gamestate State = Gamestate.Welcome;

        //variables
        
        //player
        float playerDmg = 10.0f, playerHp = 150.0f, weaponDmg = 0, armor = 0, playerSpeed = 200.0f, bulletSpeed = 600.0f;
        Vector2 playerPos = new Vector2((screenWidth-70)/2, (screenHeight-70)/2);
        Vector2 playerCenter = new Vector2(playerPos.X + (70 / 2), playerPos.Y + (70 / 2));

        //game
        float lvl = 1f, towerHp = 100f;
        int HowToPlayLvl = 1, enemiesInLevel = 0, whichPath = 1;
        float path1GenX = screenWidth / 3, path2GenX = screenWidth / 3 * 2, path1GenY = 0, path2GenY = 0;
        Vector2 cursorPos = Raylib.GetMousePosition();
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemies = new List<Enemy>();
        List<EnemyBullet> enemyBullets = new List<EnemyBullet>();
        List<Vector2> path1 = new List<Vector2>();
        List<Vector2> path2 = new List<Vector2>();
        float textTimer = 0.1f;
        float enemyWaitTime = 1f, LevelWaitTime = 1f;
        float FrameTime = Math.Min(Raylib.GetFrameTime(), 0.05f);
        bool isPath1Generated = false, isPath2Generated = false, isNextLevel = true, Xturn = false;

        //code
        while (!Raylib.WindowShouldClose())
        {
            cursorPos = Raylib.GetMousePosition();
            FrameTime = Raylib.GetFrameTime(); 
            playerCenter = new Vector2(playerPos.X + (70 / 2), playerPos.Y + (70 / 2));

            //Code part
            switch (State)
            {
                case Gamestate.Welcome:
                    break;
                case Gamestate.HowToPlay:
                    movement();
                    if (HowToPlayLvl == 1 && (Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.A) || Raylib.IsKeyPressed(KeyboardKey.S) || Raylib.IsKeyPressed(KeyboardKey.D)))
                    {
                        HowToPlayLvl++;
                        textTimer = 1.0f;
                        isNextLevel = true;
                    }
                    if (HowToPlayLvl > 1)
                    {
                        shooting();
                        if (HowToPlayLvl == 2 && Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            HowToPlayLvl++;
                            textTimer = 1.0f;
                            isNextLevel = true;
                        }
                    }
                    if (HowToPlayLvl == 3 && isPath1Generated && isPath2Generated)
                    {
                        if (enemiesInLevel < 10)
                        {
                            Random rand = new Random();
                            whichPath = rand.Next(1, 3);
                            enemySpawn();
                        }
                        enemyMovement();
                    }
                    enemyShooting();
                    
                    break;
            }



            //Draw part
            Raylib.BeginDrawing();
            switch (State)
            {


                //welcome screen
                case Gamestate.Welcome:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText("DEFENSIVA DE TOWER", (screenWidth - Raylib.MeasureText("DEFENSIVA DE TOWER", 60))/2, (screenHeight - 60)/2 - 300, 60, Color.SkyBlue);
                    Rectangle Play = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, Play))
                    {
                        Raylib.DrawRectangleRec(Play, Color.DarkGreen);
                        Raylib.DrawText("Play", (screenWidth - Raylib.MeasureText("Play", 44))/2, (screenHeight - 44)/2, 44, Color.White);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.Upgrade;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(Play, Color.Green);
                        Raylib.DrawText("Play", (screenWidth - Raylib.MeasureText("Play", 40))/2, (screenHeight - 40)/2, 40, Color.DarkGray);
                    }



                    Rectangle HowToPlay = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2 + 130, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, HowToPlay))
                    {
                        Raylib.DrawRectangleRec(HowToPlay, Color.DarkGray);
                        Raylib.DrawText("How to play", (screenWidth - Raylib.MeasureText("How to play", 39))/2, (screenHeight - 39)/2 + 130, 39, Color.White);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.HowToPlay;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(HowToPlay, Color.LightGray);
                        Raylib.DrawText("How to play", (screenWidth - Raylib.MeasureText("How to play", 36))/2, (screenHeight - 36)/2 + 130, 36, Color.Black);
                    }



                    Rectangle Exit = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2 + 260, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, Exit))
                    {
                        Raylib.DrawRectangleRec(Exit, DarkRed);
                        Raylib.DrawText("Exit", (screenWidth - Raylib.MeasureText("Exit", 46))/2, (screenHeight - 46)/2 + 260, 46, Color.SkyBlue);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.Exit;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(Exit, Color.Red);
                        Raylib.DrawText("Exit", (screenWidth - Raylib.MeasureText("Exit", 40))/2, (screenHeight - 40)/2 + 260, 40, Color.White);
                    }

                    Vector2[] WelcomePath1 = new Vector2[] {new Vector2(150, 0), new Vector2(150, 200), new Vector2(300, 200), new Vector2(300, 520), new Vector2(100, 520), new Vector2(100, 800), new Vector2(200, 800), new Vector2(200, 1080)};
                    Raylib.DrawSplineLinear(WelcomePath1, WelcomePath1.Length, 50.0f, Color.DarkGray);
                    for (int i = 1; i < WelcomePath1.Length - 1; i++)
                    {
                        Raylib.DrawRectangleV(new Vector2(WelcomePath1[i].X - 50.0f / 2, WelcomePath1[i].Y - 50.0f / 2), new Vector2(50.0f, 50.0f), Color.DarkGray);
                    }


                    Vector2[] WelcomePath2 = new Vector2[] {new Vector2(1800, 0), new Vector2(1800, 200), new Vector2(1620, 200), new Vector2(1620, 460), new Vector2(1540, 460), new Vector2(1540, 320), new Vector2(1440, 320), new Vector2(1440, 760), new Vector2(1650, 760), new Vector2(1650, 1080)};
                    Raylib.DrawSplineLinear(WelcomePath2, WelcomePath2.Length, 50.0f, Color.DarkGray);
                    for (int i = 1; i < WelcomePath2.Length - 1; i++)
                    {
                        Raylib.DrawRectangleV(new Vector2(WelcomePath2[i].X - 50.0f / 2, WelcomePath2[i].Y - 50.0f / 2), new Vector2(50.0f, 50.0f), Color.DarkGray);
                    }

                    break;


                //How to play
                case Gamestate.HowToPlay:
                    Raylib.ClearBackground(Color.Black);
                    switch(HowToPlayLvl)
                    {
                        case 1:
                            Raylib.DrawText("Movement: W, S, A, D", (screenWidth - Raylib.MeasureText("Movement: W, S, A, D", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                            break;
                        case 2:
                            if (textTimer > 0)
                            {
                               Raylib.DrawText("Movement: W, S, A, D", (screenWidth - Raylib.MeasureText("Movement: W, S, A, D", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                               textTimer -= FrameTime;
                            }
                            else if (textTimer <= 0)
                            {
                                Raylib.DrawText("Shooting: LMB", (screenWidth - Raylib.MeasureText("Shooting: LMB", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                            }
                            break;
                        case 3:
                            if (textTimer > 0)
                            {
                               Raylib.DrawText("Shooting: LMB", (screenWidth - Raylib.MeasureText("Shooting: LMB", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                               textTimer -= FrameTime;
                            }
                            else if (textTimer <= 0)
                            {
                                Raylib.DrawText("Enemies: Red squares", (screenWidth - Raylib.MeasureText("Enemies: Red squares", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                                if (isNextLevel)
                                {
                                    isPath1Generated = false;
                                    isPath2Generated = false;
                                    isNextLevel = false;
                                    LevelWaitTime = 3f;
                                }
                                if (!isPath1Generated)
                                {
                                    pathGeneration1();
                                }
                                if (!isPath2Generated)
                                {
                                    pathGeneration2();
                                }
                                Raylib.DrawSplineLinear(path1.ToArray(), path1.Count, 70.0f, Color.DarkGray);
                                for (int i = 1; i < path1.Count - 1; i++)
                                {
                                    Raylib.DrawRectangleV(new Vector2(path1[i].X - 70.0f / 2, path1[i].Y - 70.0f / 2), new Vector2(70.0f, 70.0f), Color.DarkGray);
                                }
                                Raylib.DrawSplineLinear(path2.ToArray(), path2.Count, 70.0f, Color.DarkGray);
                                for (int i = 1; i < path2.Count - 1; i++)
                                {
                                    Raylib.DrawRectangleV(new Vector2(path2[i].X - 70.0f / 2, path2[i].Y - 70.0f / 2), new Vector2(70.0f, 70.0f), Color.DarkGray);
                                }
                            }
                            break;
                    }
                    foreach (var bullet in bullets)
                    {
                        Raylib.DrawRectangleRec(bullet.Rect, Color.DarkGreen);
                    }
                    Raylib.DrawRectangle((int)playerPos.X, (int)playerPos.Y, 70, 70, Color.Green);
                    foreach (var enemy in enemies)
                    {
                        if (enemy.isHit)
                        {
                            Raylib.DrawRectangleRec(enemy.Rect, DarkRed);
                        }
                        else
                        {
                            Raylib.DrawRectangleRec(enemy.Rect, Color.Red);
                        }
                    }
                    foreach (var bullet in enemyBullets)
                    {
                        Raylib.DrawRectangleRec(bullet.Rect, Color.Maroon);
                    }
                    break;


                //exit
                case Gamestate.Exit:
                    Raylib.CloseWindow();
                    break;


                //Upgrade 
                case Gamestate.Upgrade:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText("Not done bro.", (screenWidth - Raylib.MeasureText("Not done bro.", 40))/2, (screenHeight - 40)/2, 40, Color.White);
                    break;

            }
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();




        void movement()
        {
            if(Raylib.IsKeyDown(KeyboardKey.W)) {if(playerPos.Y > 0) {playerPos.Y -= playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.S)) {if(playerPos.Y < screenHeight - 70) {playerPos.Y += playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.A)) {if(playerPos.X > 0) {playerPos.X -= playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.D)) {if(playerPos.X < screenWidth - 70) {playerPos.X += playerSpeed * FrameTime;}}
        }
        void shooting()
        {
            if(Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
            Vector2 BulletDirection = Vector2.Normalize(cursorPos - playerCenter);
            bullets.Add(new Bullet {Rect = new Rectangle(playerCenter.X - 5, playerCenter.Y - 5, 10, 10), Velocity = BulletDirection * bulletSpeed});
            }
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = bullets[i];
                bullet.Rect.X += bullet.Velocity.X * FrameTime; 
                bullet.Rect.Y += bullet.Velocity.Y * FrameTime;
                bullets[i] = bullet;
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    Enemy enemy = enemies[j];
                    if (Raylib.CheckCollisionRecs(bullet.Rect, enemy.Rect))
                    {
                        enemy.isHit = true;
                        enemy.hitTimer = 0.1f;
                        enemy.enemyHp -= playerDmg + weaponDmg;
                        enemies[j] = enemy;
                        if (enemy.enemyHp <= 0)
                        {
                            enemies.RemoveAt(j);
                        }
                        bullets.RemoveAt(i);
                        break;
                    }
                }
            }  
        }
        void enemySpawn()
        {
            if (enemyWaitTime <= 0)
            {
                List<Vector2> selectedPath = whichPath == 1 ? path1 : path2;
                enemies.Add(new Enemy { Rect = new Rectangle(selectedPath[0].X - 25, selectedPath[0].Y, 50, 50), enemyLvl = lvl, enemyHp = 50 * (float)Math.Sqrt(lvl), enemyDmg = 5 * (float)Math.Sqrt(lvl), enemySpd = 100, path = selectedPath, NextPointIndex = 1, isHit = false, hitTimer = 0 });   
                enemyWaitTime = LevelWaitTime;
                enemiesInLevel++;
            }
            else enemyWaitTime -= FrameTime;
        }
        void enemyMovement()
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy.isHit)
                {
                    enemy.hitTimer -= FrameTime;
                    if (enemy.hitTimer <= 0)
                    {
                        enemy.isHit = false;
                    }
                }
                Vector2 enemyPos = new Vector2(enemy.Rect.X + 25, enemy.Rect.Y + 25);
                Vector2 targetPos = enemy.path[enemy.NextPointIndex];
                Vector2 toTarget = targetPos - enemyPos;
                float step = enemy.enemySpd * FrameTime;
                if (toTarget.Length() <= step)
                {
                    enemyPos = targetPos;
                    enemy.NextPointIndex++;
                    if (enemy.NextPointIndex >= enemy.path.Count)
                    {
                        enemies.RemoveAt(i);
                        continue;
                    }
                }
                else
                {
                    enemyPos += Vector2.Normalize(toTarget) * step;
                }
                enemy.Rect.X = enemyPos.X - 25;
                enemy.Rect.Y = enemyPos.Y - 25;
                enemies[i] = enemy;
            }
        }
        void enemyShooting()
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (enemy.shootTimer <= 0)
                {
                    Vector2 BulletDirection = Vector2.Normalize(playerCenter - new Vector2(enemy.Rect.X + 25, enemy.Rect.Y + 25));
                    enemyBullets.Add(new EnemyBullet {Rect = new Rectangle(enemy.Rect.X + 20, enemy.Rect.Y + 20, 10, 10), Velocity = BulletDirection * enemy.enemySpd * 2, damage = enemy.enemyDmg});
                    enemy.shootTimer = 2f;
                }
                else
                {
                    enemy.shootTimer -= FrameTime;
                }
                enemies[i] = enemy;
            }
            for (int j = enemyBullets.Count - 1; j >= 0; j--)
            {
                EnemyBullet bullet = enemyBullets[j];
                bullet.Rect.X += bullet.Velocity.X * FrameTime; 
                bullet.Rect.Y += bullet.Velocity.Y * FrameTime;
                enemyBullets[j] = bullet;
                if (Raylib.CheckCollisionRecs(bullet.Rect, new Rectangle(playerPos.X, playerPos.Y, 70, 70)))
                {
                    playerHp -= bullet.damage - armor;
                    enemyBullets.RemoveAt(j);
                    break;
                }
            }
        }
        void pathGeneration1()
        {
            Random rand1 = new Random();
            Random rand2 = new Random();
            path1.Add(new Vector2(path1GenX, path1GenY));
            Xturn = false;
            while (path1GenY < screenHeight)
            {
                if (Xturn)
                {
                    path1GenX = rand1.Next(1, screenWidth / 300) * 100;
                    Xturn = false;
                }
                else
                {
                    path1GenY += rand2.Next(3, 8) * 50;
                    if (path1GenY < 0)
                    {
                        path1GenY = 0;
                    }
                    if (path1GenY > screenHeight)
                    {
                        path1GenY = screenHeight;
                    }
                    Xturn = true;
                }
                path1.Add(new Vector2(path1GenX, path1GenY));
            }
            path1GenX = screenWidth / 3;
            path1GenY = 0;
            isPath1Generated = true;
        }
        void pathGeneration2()
        {
            Random rand1 = new Random();
            Random rand2 = new Random();
            path2.Add(new Vector2(path2GenX, path2GenY));
            Xturn = false;
            while (path2GenY < screenHeight)
            {
                if (Xturn)
                {
                    path2GenX = rand1.Next(screenWidth / 150, screenWidth / 100 - 1) * 100;
                    Xturn = false;
                }
                else
                {
                    path2GenY += rand2.Next(3, 8) * 50;
                    if (path2GenY < 0)
                    {
                        path2GenY = 0;
                    }
                    if (path2GenY > screenHeight)
                    {
                        path2GenY = screenHeight;
                    }
                    Xturn = true;
                }
                path2.Add(new Vector2(path2GenX, path2GenY));
            }
            path2GenX = screenWidth / 3 * 2;
            path2GenY = 0;
            isPath2Generated = true;
        }
    }
}