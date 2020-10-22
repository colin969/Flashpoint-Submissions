import React from 'react';
import { getSubmission } from '../../services/SubmissionService';
import { Submission } from '../../types/Submission';
import { User } from '../../types/User';
import { SubmissionBox } from '../SubmissionBox';
import { UserPreview } from '../UserPreview';

export type SubmissionPageProps = {
    user?: User;
    match: {params:{subId: string}}
};

type SubmissionPageState = {
    submissionId: string;
    headerMsg?: string;
    submission?: Submission;
}

export class SubmissionPage extends React.Component<SubmissionPageProps, SubmissionPageState> {
    constructor(props: SubmissionPageProps) {
        super(props);
        this.state = {
            submissionId: this.props.match.params.subId
        };
    }

    componentDidMount() {
        getSubmission(this.state.submissionId)
        .then((sub) => this.setState({ submission: sub, headerMsg: `ID ${sub.id}`}))
        .catch((err) => this.setState({ headerMsg: `Failed to fetch ID ${this.state.submissionId} - ${err}`}));
    }

    render() {
        return (
            <div>
                <div className='submission-header'>
                    <h1>Submission</h1>
                    { this.props.user ? (
                        <UserPreview user={this.props.user}/>
                    ) : undefined }
                </div>
                { this.state.headerMsg && (
                    <h2>{this.state.headerMsg}</h2>
                )}
                { this.state.submission && this.props.user && (
                    <SubmissionBox
                        autoExpand={true}
                        userId={this.props.user ? this.props.user.id : ''}
                        roles={this.props.user ? this.props.user.roles : []} 
                        submission={this.state.submission} />
                )}
            </div>
        )
    }
}