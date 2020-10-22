namespace website {
  public class ApiResponse {
    public string message { get; set; }
    public int total { get; set; }
    public int count { get; set; }
    public int limit { get; set; }
    public int offset { get; set; }
    public object data { get; set; }

    public ApiResponse() {}
    public ApiResponse(int count, int total, int limit, int offset, object data, string message = "") {
      this.message = message;
      this.count = count;
      this.total = total;
      this.limit = limit;
      this.offset = offset;
      this.data = data;
    }
  }
}