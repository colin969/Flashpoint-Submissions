import axios from 'axios'
import { BASE_URL } from '../AuthConstants';

export default axios.create({
  baseURL: BASE_URL
});