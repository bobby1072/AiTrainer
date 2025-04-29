import { z } from "zod";

export default class Guid {
  private static readonly _uuidSchema = z.string().uuid();
  private readonly _actualValue: string;
  private constructor(guidVal?: string) {
    this._actualValue = guidVal || Guid.GenerateUUIDv4();
  }
  public toString(): string {
    return this._actualValue;
  }
  public ToString(): string {
    return this._actualValue;
  }
  public static NewGuid(): Guid {
    return new Guid();
  }
  public static NewGuidString(): string {
    return this.NewGuid().ToString();
  }
  public static Parse(uuid: string): Guid {
    if (!Guid.IsValidUUID(uuid)) {
      throw new Error("Invalid UUID v4");
    }
    return new Guid(uuid);
  }
  public static TryParse(uuid: string): Guid | undefined | null {
    try {
      return Guid.Parse(uuid);
    } catch {
      return null;
    }
  }
  public static IsValidUUID(uuid: string): boolean {
    return Guid._uuidSchema.safeParse(uuid).success;
  }
  private static GenerateUUIDv4(): string {
    const randomBytes = new Uint8Array(16);
    crypto.getRandomValues(randomBytes);

    randomBytes[6] = (randomBytes[6]! & 0x0f) | 0x40;
    randomBytes[8] = (randomBytes[8]! & 0x3f) | 0x80;
    const finalGuid = [...randomBytes]
      .map((byte, i) => {
        const hex = byte.toString(16).padStart(2, "0");
        return [4, 6, 8, 10].includes(i) ? `-${hex}` : hex;
      })
      .join("");
    if (!Guid.IsValidUUID(finalGuid)) {
      return Guid.GenerateUUIDv4();
    }
    return finalGuid;
  }
}
