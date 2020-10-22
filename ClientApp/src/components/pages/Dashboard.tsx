import React, { Component } from 'react';
import { deleteSubmission, getUserSubmissions } from '../../services/SubmissionService';
import { Submission } from '../../types/Submission';
import { User } from '../../types/User';
import { SubmissionBox } from '../SubmissionBox';
import { SubmissionsUploadBox } from '../SubmissionUpload';
import { UserPreview } from '../UserPreview'
import InfiniteScroll from 'react-infinite-scroll-component';

export type DashboardProps = {
  user?: User;
}

export type DashboardState = {
  submissions: Submission[];
  hasMore: boolean;
  fetching: boolean;
}

export class Dashboard extends Component<DashboardProps, DashboardState>{
  static displayName = Dashboard.name;

  constructor(props: DashboardProps) {
    super(props);
    this.state = {
      submissions: [],
      hasMore: true,
      fetching: false
    }
    if (this.props.user) {
      this.getMoreSubmissions();
    }
  }

  componentDidUpdate(prevProps: DashboardProps) {
    if (prevProps.user != this.props.user) {
      this.setState({ hasMore: true }, this.getMoreSubmissions);
    }
  }

  addSubmission = (sub: Submission) => {
    const newSubs = [...this.state.submissions || []];
    newSubs.unshift(sub);
    this.setState({ submissions: newSubs });
  }

  getMoreSubmissions = () => {
    if (this.state.fetching || !this.props.user) { return; }
    this.setState({ fetching: true });
    getUserSubmissions(this.props.user.id, {
      limit: 5,
      offset: this.state.submissions.length
    })
    .then((subs) => {
      if (subs.length === 0) {
        this.setState({ hasMore: false });
      } else {
        this.setState({ submissions: this.state.submissions.concat(subs), hasMore: true });
      }
    })
    .finally(() => {
      this.setState({ fetching: false });
    });
  }

  updateSubmission = (sub: Submission) => {
    const newSubs = [...this.state.submissions || []];
    const idx = newSubs.findIndex(s => s.id === sub.id);
    newSubs[idx] = sub;
    this.setState({ submissions: newSubs });
  }

  removeSubmission = (sub: Submission) => {
    const newSubs = [...this.state.submissions || []];
    const idx = newSubs.findIndex(s => s.id === sub.id);
    newSubs.splice(idx, 1);
    this.setState({ submissions: newSubs });
  }

  render () {
    return (
      <div className='dashboard-page'>
        <div className='dashboard-header'>
          <h1>Dashboard</h1>
          { this.props.user ? (
            <UserPreview user={this.props.user}/>
          ) : undefined }
        </div>
        <div className='dashboard-inner'>
          <SubmissionsUploadBox onSubmissionUploaded={this.addSubmission}/>
          <InfiniteScroll 
            dataLength={this.state.submissions.length}
            next={this.getMoreSubmissions}
            loader={<p style={{ textAlign: 'center' }}>Loading...</p>}
            hasMore={this.state.hasMore}
            endMessage={
              <p style={{ textAlign: 'center' }}>
                <b>End of Submissions</b>
              </p>
            } >
            { this.state.submissions.map((sub, idx) => (
              <div className='submission-box-wrapper'>
                <SubmissionBox
                  userId={this.props.user ? this.props.user.id : ''}
                  roles={this.props.user ? this.props.user.roles : []} 
                  key={idx}
                  submission={sub} 
                  onSubmissionDeleted={this.removeSubmission}
                  onSubmissionApproved={this.updateSubmission}
                  onSubmissionDenied={this.updateSubmission} />
              </div>
            ))}
          </InfiniteScroll>
        </div>
      </div>
    );
  }
}