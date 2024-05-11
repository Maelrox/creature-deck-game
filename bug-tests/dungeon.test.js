import { jest } from "@jest/globals";
import { ObjectId } from "mongodb";
import * as dungeonsDB from "../src/db/dungeons.js";

// DB Dungeons Mock Methods
jest.mock("../src/db/dungeons.js", () => {
  const { ObjectId } = require("mongodb");
  return {
    updateDungeon: jest.fn().mockImplementation({
      userId: "6636657680527768e73629a0",
      dungeon: "Mystic Cave",
      level: 1,
    }),
    getDungeonsOfUser: jest.fn().mockResolvedValue([
      {
        userId: "6636657680527768e73629a0",
        dungeon: "Mystic Cave",
        level: 4,
      },
      {
        userId: "6636657680527768e73629a0",
        dungeon: "Crystal Lake",
        level: 3,
      },
    ]),
    completeDungeon: jest.fn().mockResolvedValue({
      _id: new ObjectId("663748643f1085a1c36532d9"),
      dungeon: "Mystic Cave",
      userId: new ObjectId("6636657680527768e73629a0"),
      level: 1,
      success: true,
    }),
  };
});

import mongoConnector from "../src/db/mongoConnector";
import * as dungeonApi from "../src/api/dungeons.js";

describe("Dungeon API", () => {
  beforeAll(async () => {
    await mongoConnector.connectToDatabase();
  });
  afterAll(async () => {
    await mongoConnector.closeDatabaseConnection();
  });
  beforeEach(() => {
    // Reset the mocks before each test
    dungeonsDB.updateDungeon.mockReset();
    dungeonsDB.updateDungeon.mockResolvedValue({
      userId: "6636657680527768e73629a0",
      dungeon: "Mystic Cave",
      level: 1,
    });

    dungeonsDB.getDungeonsOfUser.mockReset();
    dungeonsDB.getDungeonsOfUser.mockResolvedValue([
      {
        userId: "6636657680527768e73629a0",
        dungeon: "Mystic Cave",
        level: 4,
      },
      {
        userId: "6636657680527768e73629a0",
        dungeon: "Crystal Lake",
        level: 3,
      },
    ]);

    dungeonsDB.completeDungeon.mockReset();
    dungeonsDB.completeDungeon.mockResolvedValue({
      _id: new ObjectId("663748643f1085a1c36532d9"),
      dungeon: "Mystic Cave",
      userId: new ObjectId("6636657680527768e73629a0"),
      level: 1,
      success: true,
    });
  });
  describe("Complete Dungeon", () => {
    it("should complete an existing dungeon or create a new one if it does not exist, and return the updated or created document", async () => {
      const userId = "6636657680527768e73629a0";
      const dungeonName = "Mystic Cave";
      const level = 1;

      const result = await dungeonApi.completeDungeon(
        userId,
        dungeonName,
        level
      );

      expect(result).toBeDefined();
      expect(result.userId).toStrictEqual(new ObjectId(userId));
      expect(result.dungeon).toBe(dungeonName);
      expect(result.level).toBe(level);
      expect(dungeonsDB.updateDungeon).toHaveBeenCalledTimes(1);
    });
  });
  describe("Get Dungeons of User", () => {
    it("should return an empty array or return the dungeons of the user", async () => {
      const result = await dungeonApi.getDungeonsOfUser(
        "6636657680527768e73629a0"
      );

      expect(result).toBeDefined();
      expect(result.success).toBe(true);
    });
  });
});
