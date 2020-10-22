import React from 'react';
import { User } from '../../types/User';

export type UsersPageProps = {};

export type UsersPageState = {
  users?: User[];
}

export class UsersPage extends React.Component<UsersPageProps, UsersPageState> {

  constructor(props: UsersPageProps) {
    super(props);
    this.state = {};
  }

  render() {
    return (
      <div className='users-page'>
        <h1>Users</h1>
      </div>
    );
  }
}