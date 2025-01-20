export default class Guid {
  private readonly _actualValue: string;
  private static readonly _uuidV4Regex =
    /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  private constructor(guidVal?: string) {
    this._actualValue = guidVal ? guidVal : Guid.GenerateUUIDv4();
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
    return new Guid().ToString();
  }
  public static Parse(uuid: string): Guid {
    if (!Guid.IsValidUUIDv4(uuid)) {
      throw new Error("Invalid UUID v4");
    }
    return new Guid(uuid);
  }
  public static TryParse(uuid: string): Guid | undefined | null {
    if (!Guid.IsValidUUIDv4(uuid)) {
      return null;
    }
    return new Guid(uuid);
  }
  public static IsValidUUIDv4(uuid: string): boolean {
    return Guid._uuidV4Regex.test(uuid);
  }
  private static GenerateUUIDv4(): string {
    const randomBytes = new Uint8Array(16);
    crypto.getRandomValues(randomBytes);

    randomBytes[6] = (randomBytes[6] & 0x0f) | 0x40;
    randomBytes[8] = (randomBytes[8] & 0x3f) | 0x80;

    return [...randomBytes]
      .map((byte, i) => {
        const hex = byte.toString(16).padStart(2, "0");
        return [4, 6, 8, 10].includes(i) ? `-${hex}` : hex;
      })
      .join("");
  }
}
