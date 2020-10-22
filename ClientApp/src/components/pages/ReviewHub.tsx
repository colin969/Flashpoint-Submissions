import React from 'react';
import { getSubmissions, getUserSubmissions } from '../../services/SubmissionService';
import { Submission } from '../../types/Submission';
import { User } from '../../types/User';
import { SubmissionBox } from '../SubmissionBox';
import { UserPreview } from '../UserPreview';
import InfiniteScroll from 'react-infinite-scroll-component';

export type ReviewHubProps = {
  user?: User;
}

export type ReviewHubState = {
  fetching: boolean;
  hasMore: boolean;
  submissions: Submission[];
  showDeleted: boolean;
  showAwaiting: boolean;
  showApproved: boolean;
  showDenied: boolean;
  ascending: boolean;
  orderByUpdated: boolean;
}

export class ReviewHub extends React.Component<ReviewHubProps, ReviewHubState> {
  constructor(props: ReviewHubProps) {
    super(props);

    this.state = {
      fetching: false,
      hasMore: true,
      submissions: [],
      showDeleted: false,
      showApproved: true,
      showDenied: true,
      showAwaiting: true,
      ascending: false,
      orderByUpdated: false,
    };
  }

  getMoreSubmissions = () => {
    if (this.state.fetching) { return; }
    this.setState({ fetching: true });
    getSubmissions({
      showAwaiting: this.state.showAwaiting,
      showApproved: this.state.showApproved,
      showDenied: this.state.showDenied,
      showDeleted: this.state.showDeleted,
      limit: 20,
      offset: this.state.submissions.length,
      ascending: this.state.ascending,
      orderByUpdated: this.state.orderByUpdated
    })
    .then((subs) => {
      if (subs.length === 0) {
        this.setState({ hasMore: false });
      } else {
        this.setState({ submissions: this.state.submissions.concat(subs) });
      }
    })
    .finally(() => {
      this.setState({ fetching: false });
    });
  }

  componentDidMount() {
    this.getMoreSubmissions();
  }

  removeSubmission = (sub: Submission) => {
    const newSubs = [...this.state.submissions || []];
    const idx = newSubs.findIndex(s => s.id === sub.id);
    newSubs.splice(idx, 1);
    this.setState({ submissions: newSubs });
  }

  updateSubmission = (sub: Submission) => {
    const newSubs = [...this.state.submissions || []];
    const idx = newSubs.findIndex(s => s.id === sub.id);
    newSubs[idx] = sub;
    this.setState({ submissions: newSubs });
  }

  optsChanged = () => {
    this.setState({ submissions: [], hasMore: true }, this.getMoreSubmissions);
  }
  toggleApproved = () => { this.setState({ showApproved: !this.state.showApproved}, this.optsChanged); }
  toggleDenied =   () => { this.setState({ showDenied:   !this.state.showDenied},   this.optsChanged); }
  toggleAwaiting = () => { this.setState({ showAwaiting: !this.state.showAwaiting}, this.optsChanged); }
  toggleDeleted =  () => { this.setState({ showDeleted:  !this.state.showDeleted},  this.optsChanged); }
  toggleAscending = () => { this.setState({ ascending: !this.state.ascending }, this.optsChanged); }
  toggleOrderByUpdated = () => { this.setState({ orderByUpdated: !this.state.orderByUpdated }, this.optsChanged); }

  render() {
    return (
      <div className='review-hub-page'>
        <div className='review-hub-header'>
          <h1>Review Hub</h1>
          <div className='review-hub-header__buttons'>
            <button className={`btn ${this.state.showApproved  ? 'btn-success' : 'btn-secondary'}`} onClick={this.toggleApproved}>Approved</button>
            <button className={`btn ${this.state.showDenied    ? 'btn-success' : 'btn-secondary'}`} onClick={this.toggleDenied}>Denied</button>
            <button className={`btn ${this.state.showAwaiting  ? 'btn-success' : 'btn-secondary'}`} onClick={this.toggleAwaiting}>Awaiting</button>
            <button className={`btn ${this.state.showDeleted   ? 'btn-success' : 'btn-secondary'}`} onClick={this.toggleDeleted}>Deleted</button>
            <button className={`btn btn-success`} onClick={this.toggleAscending}>{this.state.ascending ? 'Ascending' : 'Descending'}</button>
            <button className={`btn btn-success`} onClick={this.toggleOrderByUpdated}>{this.state.orderByUpdated ? 'By Status Update' : 'By Submission Date'}</button>            
          </div>
          { this.props.user ? (
            <UserPreview user={this.props.user}/>
          ) : undefined }
        </div>
        <div className='review-hub-inner'>
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