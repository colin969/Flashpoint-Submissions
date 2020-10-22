import { plainToClass } from 'class-transformer';
import { CallbackQueue } from '../CallbackQueue';
import { User } from '../types/User';

const userCache: Map<string, User> = new Map();
const fetchQueue: CallbackQueue = new CallbackQueue();

export async function getUser(id: string, refresh: boolean = false): Promise<User> {
  return fetchQueue.queue(() => _getUser(id, refresh));
}

async function _getUser(id: string, refresh: boolean = false): Promise<User> {
  if (!refresh) {
    const cachedUser = userCache.get(id);
    if (cachedUser) {
      return cachedUser;
    }
  }
  const res = await fetch(`api/user/${id}`);
  if (res.status !== 200) { throw new Error(`Failed to get user\nError: (${res.status}) ${res.statusText}`); }
  const json = await res.json();
  const user = plainToClass(User, json);
  userCache.set(user.id, user);
  return user;
}