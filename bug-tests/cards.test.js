import { jest } from "@jest/globals";
import cardsDB from "../src/db/cards.js";
import mongoConnector from "../src/db/mongoConnector.js";

// DB Cards Mock Methods
jest.mock("../src/db/cards.js", () => ({
  getCardsOfUser: jest.fn().mockResolvedValue(
    {
      cardId: "Bear",
      userId: "Morgox",
      xp: 3000,
    },
    {
      cardId: "Carrot",
      userId: "Morgox",
      xp: 2001,
    }
  ),
  updateCardExperience: jest.fn().mockResolvedValue([
    {
      userId: "player123",
      dungeon: "Mystic Cave",
      level: 5,
    },
    {
      userId: "player123",
      dungeon: "Crystal Lake",
      level: 3,
    },
  ]),
}));

describe("Games API", () => {
  beforeAll(async () => {
    await mongoConnector.connectToDatabase();
  });

  // Disconnect from the database after all tests have completed
  afterAll(async () => {
    await mongoConnector.closeDatabaseConnection();
  });
  describe("Update Experience", () => {
    it("should increase experience for all the player cards", async () => {
      const playerId = "6636657680527768e73629a0";
      const xp = 1;

      const result = await cardsDB.updateCardExperience(playerId, xp);

      expect(result).toBeDefined();
    });
  });
  describe("Create Card", () => {
    it("create a new card for the player", async () => {
      const playerId = "6636657680527768e73629a0";
      const xp = 1;
      const result = await cardsDB.insertCard(playerId, "Gryph", xp);
      expect(result).toBeDefined();
    });
  });
});
