import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { User } from '../types/User';
import { NavMenu } from './NavMenu';

export type LayoutProps = {
  user?: User;
}

export class Layout extends Component<LayoutProps> {
  static displayName = Layout.name;

  render () {
    return (
      <div>
        <NavMenu user={this.props.user} />
        <div className='page'>
          <Container>
            {this.props.children}
          </Container>
        </div>
      </div>
    );
  }
}
