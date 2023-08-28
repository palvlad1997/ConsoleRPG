using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

class NameTrait
{
    public string Name { get; set; }

    public NameTrait(string name)
    {
        Name = name;
    }
}

interface IUseSkill
{
    bool use(Player player);
    int getSkillNumber();
    string getName();
}

class Skill : NameTrait
{
    protected int epCost = 0;
    protected int skillNumber = 0;
    public Skill(string name, int cost) : base(name)
    {
        epCost = cost;
    }

    public string getName() => this.Name;
    public int getSkillNumber() => this.skillNumber;

    protected bool decreaseEp(Player player)
    {
        if (player.Energy >= epCost)
        {
            player.Energy -= epCost;
            return true;
        }

        return false;
    }
}

class MightyStrike : Skill, IUseSkill
{

    public MightyStrike() : base("Потужний удар", 25)
    {
        skillNumber = 100;
    }

    public bool use(Player player)
    {
        if (!decreaseEp(player))
        {
            return false;
        }

        player.BonusDamage = 30;

        return true;
    }
}

class UltimateDefence : Skill, IUseSkill
{
    public UltimateDefence() : base("Ультимативний захист", 50)
    {
        skillNumber = 200;
    }

    public bool use(Player player)
    {
        if (!decreaseEp(player))
        {
            return false;
        }

        player.BonusProtect = 80;

        return true;
    }
}

class RegenerateHp : Skill, IUseSkill
{
    public RegenerateHp() : base("Регенерація", 80)
    {
        skillNumber = 300;
    }

    public bool use(Player player)
    {
        if (!decreaseEp(player))
        {
            return false;
        }

        player.addedHealth(250);

        return true;
    }
}


class Armor : NameTrait
{
    public int Defense { get; set; }
    public int Price { get; set; }

    public Armor(string name, int defense, int price) : base(name)
    {
        Defense = defense;
        Price = price;
    }
}

class Weapon : NameTrait
{
    private int v;

    public int Damage { get; set; }
    public int Price { get; set; }

    public Weapon(string name, int damage, int price) : base(name) //,int price
    {
        Damage = damage;
        Price = price;
    }

    public Weapon(string name, int v) : base(name)
    {
        this.v = v;
    }
}

abstract class Person : NameTrait
{
    public int Health { get; set; }
    public int HealthMax { get; set; }
    public int Energy { get; set; }
    public int Level { get; set; }

    public void addedHealth(int value)
    {
        if (Health + value > HealthMax)
            Health = HealthMax;
        else
            Health += value;
    }

    public void damageHealth(int damage)
    {
        if (Health - damage < 0)
            Health = 0;
        else
            Health -= damage;
    }

    public Person(string name, int level) : base(name)
    {
        Level = level;
    }

    protected void showPersonData()
    {
        Console.WriteLine("Ім'я: " + this.Name);
        Console.WriteLine("Рівень: " + this.Level);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Здоров'я: " + this.Health + "/" + this.HealthMax);
        Console.ResetColor();
        Console.WriteLine("Енергія: " + this.Energy);
    }
}

abstract class Player : Person
{
    private List<IUseSkill> skillList = null;
    public int BonusDamage { get; set; }
    public int BonusProtect { get; set; }
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int Stamina { get; set; }
    public int ExperiencePoints { get; set; }
    public Armor EquippedArmor { get; set; }
    public Weapon EquippedWeapon { get; set; }
    public int Money { get; set; }
    public void addedExp(int exp)
    {
        this.ExperiencePoints += exp;
    }

    public Player(string name, int str, int agl, int stm) : base(name, 1)
    {
        this.Strength = str;
        this.Agility = agl;
        this.Stamina = stm;
        this.ExperiencePoints = 0;
        this.Energy = 100;
        this.skillList = new List<IUseSkill>();

        this.Money = 0;

        this.HealthMax = this.Health = 50 * this.Stamina;
    }

    private bool checkSkills(int skillNumber)
    {
        if (this.skillList.FindIndex(skill => skill.getSkillNumber() == skillNumber) < 0)
            return true;

        return false;
    }

    public void addedSkill(IUseSkill skill)
    {
        if (checkSkills(skill.getSkillNumber()))
        {
            skillList.Add(skill);
        }
    }

    public void removeSkill(int index)
    {
        skillList.RemoveAt(index);
    }

    public void useSkill(int index)
    {
        if (index >= skillList.Count)
        {
            throw new Exception("Такого вміння немає!");
        }

        skillList[index].use(this);
    }

    public void resetPlayerValue()
    {
        BonusDamage = 0;
        BonusProtect = 0;

        this.addedHealth(10);
    }

    public void showSkillList()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            Console.WriteLine(i + ". " + skillList[i].getName());
        }
    }

    public void EquipArmor(Armor armor)
    {
        this.EquippedArmor = armor;
    }

    public void EquipWeapon(Weapon weapon)
    {
        this.EquippedWeapon = weapon;
    }

    public void ShowInfo()
    {
        Console.WriteLine();
        Console.WriteLine("Характеристики гравця:");
        showPersonData();
        Console.WriteLine("Сила: " + this.Strength);
        Console.WriteLine("Спритність: " + this.Agility);
        Console.WriteLine("Витривалість: " + this.Stamina);
        Console.WriteLine("Очки досвіду: " + this.ExperiencePoints);
        Console.Write("Зброя: " + (this.EquippedWeapon != null ? this.EquippedWeapon.Name : "Відсутня"));

        if (this.EquippedWeapon != null)
            Console.Write(" (Ціна: " + this.EquippedWeapon.Price + ")");

        Console.Write("\nБроня: " + (this.EquippedArmor != null ? this.EquippedArmor.Name : "Відсутня"));

        if (this.EquippedArmor != null)
            Console.Write(" (Ціна: " + this.EquippedArmor.Price + ")");

        Console.WriteLine();
    }

    public int CalculateDamage()
    {
        int baseDamage = this.Strength + this.EquippedWeapon?.Damage ?? 0;
        int criticalStrikeChance = this.Agility;
        bool isCritical = new Random().Next(0, 100) < criticalStrikeChance;
        int totalDamage = isCritical ? baseDamage * 2 : baseDamage;

        return totalDamage + BonusDamage;
    }

    public int CalculateDefense()
    {
        int baseDefense = this.Stamina + this.EquippedArmor?.Defense ?? 0;
        return baseDefense + BonusProtect;
    }
}

class Barbarian : Player
{
    public Barbarian(string name)
        : base(name, 30, 10, 20)
    {
    }
}

class Tank : Player
{
    public Tank(string name)
        : base(name, 20, 10, 30)
    {
    }
}

class Rogue : Player
{
    public Rogue(string name)
        : base(name, 10, 30, 20)
    {
    }
}

class Monster : Person
{
    public int Damage { get; set; }
    public int Armor { get; set; }
    public int ExperiencePoints { get; set; }

    public Monster(Random random, string name, int level) : base(name, level)
    {
        HealthMax = Health = random.Next(50, 100) * Level;
        Energy = random.Next(0, 50) * Level;
        Damage = random.Next(5, 15) * Level;
        Armor = random.Next(1, 5) * Level;
        ExperiencePoints = 20 * Level;
    }

    public void showInfo()
    {
        Console.WriteLine("Характеристики монстра:");
        showPersonData();
        Console.WriteLine("Шкода: " + this.Damage);
        Console.WriteLine("Броня: " + this.Armor);
        Console.WriteLine("Очки досвіду: " + this.ExperiencePoints);
    }
}

class Engine
{
    private List<string> monsterNames = null;
    List<string> armorNames = null;
    List<string> weaponNames = null;
    private Random random = null;


    public Engine(Random random)
    {
        monsterNames = new List<string>
        {
            "Монстр1",
            "Монстр2",
            "Монстр3",
            "Монстр4",
            "Монстр5",
            "Монстр6",
            "Монстр7",
            "Монстр8",
            "Монстр9",
            "Монстр10"
        };

        armorNames = new List<string>
        {
            "Лати",
            "Кольчуга",
            "Кіраса",
            "Шолом",
            "Нагрудник"
        };

        weaponNames = new List<string>
        {
            "Меч",
            "Сокира",
            "Кинджал",
            "Лук",
            "Палиця"
        };

        this.random = random;
    }

    public Monster GenerateMonster(int playerLevel)
    {
        if (playerLevel == 1)
            playerLevel = random.Next(playerLevel, playerLevel + 2);
        else
            playerLevel = random.Next(playerLevel - 1, playerLevel + 2);

        return new Monster(this.random, GetRandomMonsterName(), playerLevel);
    }

    private string GetRandomMonsterName()
    {
        int index = random.Next(monsterNames.Count);
        string name = monsterNames[index];
        monsterNames.RemoveAt(index);
        return name;
    }

    public Player CreatePlayer(string playerName, string playerClass)
    {
        Player player = null;

        switch (playerClass.ToLower())
        {
            case "варвар":

                player = new Barbarian(playerName);
                player.EquipWeapon(new Weapon("Іржава Сокира", 3));
                break;
            case "танк":
                player = new Tank(playerName);
                player.EquipWeapon(new Weapon("Тупий Меч", 2));
                break;
            case "розбишака":
                player = new Rogue(playerName);
                player.EquipWeapon(new Weapon("Отруйне Зубило", 2));
                break;
            default:
                throw new Exception("Такого Класу немає");
        };

        return player;
    }

    public Armor GenerateArmor()
    {
        int defense = random.Next(3, 20);
        int price = random.Next(10, 100);

        if (armorNames.Count == 0)
            throw new Exception("Назви для броні закінчились");

        int index = random.Next(armorNames.Count);
        string name = armorNames[index];
        armorNames.RemoveAt(index);

        return new Armor(name, defense, price);
    }



    public Weapon GenerateWeapon(string name = "", int damage = 0)
    {
        if (name.Length == 0)
        {
            int index = random.Next(weaponNames.Count);
            name = weaponNames[index];
            weaponNames.RemoveAt(index);
        }

        if (damage == 0)
            damage = random.Next(3, 20);

        int price = damage * random.Next(5, 20);

        return new Weapon(name, damage, price); //add ,price
    }

    static class Logger
    {
        static public void showVictoryMessage(int exp)
        {
            Console.WriteLine($"Ви перемогли і отримали {exp} очків досвіду");
        }

        static public void showOnePunchResult(string name, int damage)
        {
            Console.WriteLine($"{name} наніс {damage} шкоди");
        }
    }

    class EventRoom
    {
        private Random random;
        private Engine engine;

        public EventRoom(Random random, Engine engine)
        {
            this.random = random;
            this.engine = engine;
        }

        public void Enter(Player player)
        {
            int eventType = random.Next(1, 4);

            switch (eventType)
            {
                case 1:
                    BattleEvent(player);
                    break;
                case 2:
                    RandomEvent(player);
                    break;
                case 3:
                    EmptyRoomEvent(player);
                    break;
            }
        }

        private void BattleEvent(Player player)
        {
            Console.WriteLine("Ви зустріли монстра!");
            Console.WriteLine("Починаємо бій...");

            new Events(random).battle(player, this.engine.GenerateMonster(player.Level));
        }

        private void RandomEvent(Player player)
        {
            int randomValue = random.Next(1, 4);

            switch (randomValue)
            {
                case 1:
                    Console.WriteLine("Збільшення характеристик гравця!");
                    player.Strength += 5;
                    player.Agility += 5;
                    player.Stamina += 5;
                    break;
                case 2:
                    Console.WriteLine("Зменшення характеристик гравця!");
                    player.Strength -= 5;
                    player.Agility -= 5;
                    player.Stamina -= 5;
                    break;
                case 3:
                    Console.WriteLine("Пастка! Ви зазнали шкоди.");
                    player.damageHealth(10);
                    break;
            }
        }

        private void EmptyRoomEvent(Player player)
        {
            Console.WriteLine("Ця кімната порожня. Нічого цікавого не знайдено.");
        }
    }

    class Events
    {
        Random random = null;

        public Events(Random random)
        {
            this.random = random;
        }

        private int calculateDamage(int damage, int armor)
        {
            if (damage - armor < 0)
                damage = 0;
            else
                damage -= armor;

            return damage;
        }

        public void battle(Player player, Monster monster)
        {
            while (player.Health > 0 && monster.Health > 0)
            {
                Console.Clear();
                player.ShowInfo();
                monster.showInfo();

                Console.WriteLine("===========================");
                Console.ForegroundColor = ConsoleColor.Red;

                int damage = calculateDamage(player.CalculateDamage(), monster.Armor);
                Logger.showOnePunchResult(player.Name, damage);
                monster.damageHealth(damage);

                damage = calculateDamage(monster.Damage, player.CalculateDefense());
                Logger.showOnePunchResult(monster.Name, damage);
                player.damageHealth(damage);

                Console.ResetColor();
                // Console.ReadLine();
                Thread.Sleep(500);
            }

            if (monster.Health == 0)
            {
                Logger.showVictoryMessage(monster.ExperiencePoints);
                player.addedExp(monster.ExperiencePoints);
                player.Money += monster.ExperiencePoints * 10;
            }
            else
            {
                Console.WriteLine("WASTED!!!");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Random random = new Random();
            Engine engine = new Engine(random);
            EventRoom eventRoom = new EventRoom(random, engine);

            Console.Write("Введіть ім'я гравця: ");
            string playerName = Console.ReadLine();

            Console.Write("Виберіть клас гравця (варвар, танк, розбишака): ");
            string playerClass = Console.ReadLine();

            Player player = engine.CreatePlayer(playerName, playerClass);
            if (player == null)
            {
                Console.WriteLine("Не вдалося створити гравця.");
                return;
            }

            player.EquipArmor(engine.GenerateArmor());
            player.EquipWeapon(engine.GenerateWeapon());

            Monster monster = engine.GenerateMonster(player.Level);

            while (player.Health > 0)
            {
                Console.WriteLine("Ви потрапили в кімнату...");
                eventRoom.Enter(player);

                if (player.Health > 0)
                {
                    Console.WriteLine("Продовжуємо подорож...\n");
                    Thread.Sleep(2000);
                }
            }

        }
    }
}