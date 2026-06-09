import { describe, expect, it } from "vitest";
import { loginSchema, registerSchema } from "./authSchemas";

describe("auth schemas", () => {
  it("validates login email and password", () => {
    expect(loginSchema.safeParse({ email: "abner@example.com", password: "StrongPassword123!" }).success).toBe(true);
    expect(loginSchema.safeParse({ email: "invalid", password: "" }).success).toBe(false);
  });

  it("validates register name, email and minimum password length", () => {
    expect(
      registerSchema.safeParse({
        name: "Abner Suhett",
        email: "abner@example.com",
        password: "StrongPassword123!",
      }).success,
    ).toBe(true);
    expect(registerSchema.safeParse({ name: "", email: "invalid", password: "short" }).success).toBe(false);
  });
});
