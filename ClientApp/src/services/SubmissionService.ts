import { plainToClass } from 'class-transformer';
import { Meta } from '../types/Meta';
import { Submission } from '../types/Submission';
import http from './ApiCommon';

export async function getUserSubmissions(userId: string, opts?: GetSubmissionOpts): Promise<Submission[]> {
  const submissions: Submission[] = [];
  const res = await http.get(`api/submissions/${userId}`, { params: opts });
  if (res.status !== 200) { throw new Error(`Failed to fetch User Submissions\nError: (${res.status}) ${res.statusText}`); }
  const json = res.data;
  for (const sub of json['data']) {
    submissions.push(plainToClass(Submission, sub));
  }
  return submissions;
}

export type GetSubmissionOpts = {
  showDeleted?: boolean;
  showApproved?: boolean;
  showDenied?: boolean;
  showAwaiting?: boolean;
  limit?: number;
  offset?: number;
  ascending?: boolean;
  orderByUpdated?: boolean;
}

export async function getSubmissions(opts?: GetSubmissionOpts): Promise<Submission[]> {
  const submissions: Submission[] = [];
  const res = await http.get(`api/submissions`, { params: opts });
  if (res.status !== 200) { throw new Error(`Failed to fetch Submissions\nError: (${res.status}) ${res.statusText}`); }
  const json = res.data;
  for (const sub of json['data']) {
    submissions.push(plainToClass(Submission, sub));
  }
  return submissions;
}

export async function getSubmission(id: string): Promise<Submission> {
  const res = await http.get(`api/submission/${id}`);
  if (res.status !== 200) { throw new Error(`Failed to get submission\nError: (${res.status}) ${res.statusText}`)}
  return plainToClass(Submission, res.data);
}

export async function getSubmissionMeta(id: string): Promise<Meta> {
  const res = await http.get(`api/submission/${id}/meta`);
  if (res.status !== 200) { throw new Error(`Failed to get submission meta\nError: (${res.status}) ${res.statusText}`)}
  return plainToClass(Meta, res.data);
}

export async function deleteSubmission(id: string): Promise<Submission> {
  const res = await http.delete(`api/submission/${id}`);
  if (res.status !== 200) { throw new Error(`Failed to delete submission\nError: (${res.status}) ${res.statusText}`)}
  return plainToClass(Submission, res.data);
}

export async function updateSubmission(sub: Submission): Promise<Submission> {
  const res = await http.put(`api/submission/${sub.id}`, sub);
  if (res.status !== 200) { throw new Error(`Failed to delete submission\nError: (${res.status}) ${res.statusText}`)}
  return plainToClass(Submission, res.data);
}

export async function uploadFile(index: number, file: string | Blob, onProgress: ((progressEvent: ProgressEvent<EventTarget>) => void) | undefined) {
  const formData = new FormData();
  formData.append("file", file);

  return http.post('api/submission/', formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
    onUploadProgress: onProgress,
  })
}