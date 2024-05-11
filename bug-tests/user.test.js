import { jest } from "@jest/globals";
import userDB from "../src/db/user.js";
import mongoConnector from "../src/db/mongoConnector.js";

jest.mock("../src/db/user.js", () => ({
  registerUser: jest.fn().mockResolvedValue({
    userId: "Killer",
    password: "Morgox",
  }),
}));

describe("Games API", () => {
  beforeAll(async () => {
    await mongoConnector.connectToDatabase();
  });

  afterAll(async () => {
    await mongoConnector.closeDatabaseConnection();
  });
  describe("Register one user", () => {
    it("should create an user in the database", async () => {
      const userId = "Killer";
      const password = "Morgox";
      const result = await userDB.registerUser(userId, password);
      expect(result).toBeDefined();
      expect(result.acknowledge.toBeDefined());
    });
  });
});
