export class CallbackQueue {

  _queue: (() => Promise<any>)[];
  _current?: Promise<any>;

  constructor() {
    this._queue = [];
  }

  async queue<T>(func: () => T): Promise<T> {
    return new Promise<T>((resolve, reject) => {
      const callback = async () => {
        Promise.resolve(func())
        .then((ret) => resolve(ret))
        .catch(() => reject())
        .finally(() => {
          this._current = undefined;
          const next = this._queue.shift();
          if (next) {
            this._current = next();
          }
        });
      };
      if (!this._current) {
        this._current = callback();
      } else {
        this._queue.push(callback);
      }
    }); 
  }
}